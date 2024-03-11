using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Presentation;
using LinqToDB.SqlQuery;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Transaction;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Transactions
{
    public partial class SendProfitTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly TransactionSettings _transactionSettings;
        private readonly ITransactionService _transactionService;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IWithdrawService _withdrawService;
        private readonly IWorkflowMessageService _workflowMessageService;

        #endregion

        #region Ctor

        public SendProfitTask(ILogger logger,
            TransactionSettings transactionSettings,
            ITransactionService transactionService,
            ICustomerService customerService,
            ISettingService settingService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            IWithdrawService withdrawService,
            IWorkflowMessageService workflowMessageService)
        {
            _logger = logger;
            _transactionSettings = transactionSettings;
            _transactionService = transactionService;
            _customerService = customerService;
            _settingService = settingService;
            _localizationService = localizationService;
            _workContext = workContext;
            _withdrawService = withdrawService;
            _workflowMessageService = workflowMessageService;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var dateTime = DateTime.Now;
            //runs on the 1st day of the month (_transactionSettings.InvestmentDateStart)
            if (DateTime.Now.Day.Equals(_transactionSettings.InvestmentDateStart) ||
                await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            {
                var customers = await (await _customerService.GetAllCustomersAsync(dontInvestAmount: false))
                    .WhereAwait(async c => await _customerService.IsRegisteredAsync(c) &&
                        c.InvestedAmount > decimal.Zero).ToListAsync();

                foreach (var customer in customers)
                {
                    if (customer.LastReturnDate.GetValueOrDefault().Month.Equals(dateTime.Month) &&
                        customer.LastReturnDate.GetValueOrDefault().Year.Equals(dateTime.Year) &&
                        !_transactionSettings.ForecfullyGiveReturn)
                        continue;

                    try
                    {
                        //percentage on return come from external API 
                        var apiResponse = await _transactionService.GetReturnPercentageOfCustomerTransactionsAsync(customerCommission: customer.CommissionToHouse);
                        var returnPercentage = apiResponse.investorInterestPercentage;

                        //calculate return amount on the invest amount
                        var returnOnInvestedAmount = customer.InvestedAmount * (returnPercentage / 100);
                        //calculate commission on the invest amount
                        var commissionOnReturnAmount = customer.InvestedAmount * ((apiResponse.totalGeneratedPercentage - apiResponse.investorInterestPercentage) / 100);

                        int returnTransactionFkId = default;

                        #region Deposit

                        //deposit the amount into the customer wallet
                        var debitTransaction = new Transaction()
                        {
                            CustomerId = customer.Id,
                            TransactionAmount = returnOnInvestedAmount,
                            TransactionType = TransactionType.Credit,
                            Status = Status.Pending,
                            TransactionNote = "Return: " + await _localizationService.GetResourceAsync("Customer.Invest.ReturnAmount"),
                        };

                        await _transactionService.InsertTransactionAsync(debitTransaction, false);
                        await _transactionService.MarkCreditTransactionAsCompletedAsync(transaction: debitTransaction, false);

                        #endregion

                        #region Commission

                        await _transactionService.InsertCommissionAsync(new Commission()
                        {
                            Amount = commissionOnReturnAmount,
                            TransactionId = debitTransaction.Id,
                            Percentage = apiResponse.totalGeneratedPercentage - apiResponse.investorInterestPercentage
                        });

                        #endregion

                        //get the return amount and invested balance to the current balance 
                        var updatedCustomer = await _customerService.GetCustomerByIdAsync(customer.Id);
                        updatedCustomer.InvestedAmount += updatedCustomer.CurrentBalance;
                        updatedCustomer.CurrentBalance = default;
                        updatedCustomer.LastReturnDate = dateTime;

                        //if customer wants to pay out the return amount then make a withdrawal request of the return amount
                        if (customer.PaymentType.Equals(PaymentTypeEnum.PayOut))
                        {
                            #region Withdraw

                            var withdrawalMethod = await _withdrawService.GetWithdrawalMethodByIdAsync(customer.DefaultWithdrawalMethodId);
                            if (!(withdrawalMethod?.IsEnabled) ?? false)
                                break;

                            withdrawalMethod.IsRequested = true;
                            withdrawalMethod = await _withdrawService.CreateDuplicateWithdrawalMethodForCustomerAsync(withdrawalMethod, customer);

                            var transaction = new Transaction()
                            {
                                CustomerId = customer.Id,
                                TransactionAmount = -returnOnInvestedAmount,
                                TransactionType = TransactionType.Debit,
                                Status = Status.Pending,
                                TransactionNote = "Withraw: " + await _localizationService.GetLocalizedEnumAsync(withdrawalMethod.Type),
                                WithdrawalMethodId = withdrawalMethod.Id,
                            };
                            await _transactionService.InsertTransactionAsync(transaction, false);
                            returnTransactionFkId = transaction.Id;

                            updatedCustomer.InvestedAmount -= returnOnInvestedAmount;

                            //send mail for payment payout
                            await _transactionService.SendEmailOnTransactionAsync(transaction);
                            
                            #endregion
                        }
                        else if (customer.PaymentType.Equals(PaymentTypeEnum.Compound))
                        {
                            //send mail for payment compound
                            await _transactionService.SendEmailOnTransactionAsync(debitTransaction);
                            returnTransactionFkId = debitTransaction.Id;
                        }

                        await _customerService.UpdateCustomerAsync(updatedCustomer);

                        #region Return Transaction

                        //make a return transaction record
                        var returnTransaction = new ReturnTransaction()
                        {
                            ReturnAmount = returnOnInvestedAmount,
                            ReturnPercentage = returnPercentage,
                            //add previous month date return will always be giving of the previous month
                            ReturnDateOnUtc = dateTime.AddMonths(-1),
                            //get debit transaction ID if payment type is compound otherise credit transaction ID
                            TransactionId = returnTransactionFkId,
                        };
                        await _transactionService.InsertReturnTransactionAsync(returnTransaction);

                        //send mail after generating return
                        await _workflowMessageService.SendReturnGeneratedAdminNotificationAsync(returnTransaction: returnTransaction, languageId: (await _workContext.GetWorkingLanguageAsync()).Id);

                        #endregion

                        //switch (customer.PaymentType)
                        //{
                        //    case PaymentTypeEnum.Compound:
                        //        {
                        //            var transaction = new Transaction()
                        //            {
                        //                CustomerId = customer.Id,
                        //                TransactionAmount = returnOnInvestedAmount,
                        //                TransactionType = TransactionType.Credit,
                        //                Status = Status.Pending,
                        //                TransactionNote = "Return: " + await _localizationService.GetResourceAsync("Customer.Invest.ReturnAmount"),
                        //                ReturnTransactionId = returnTransaction.Id
                        //            };
                        //            await _transactionService.InsertTransactionAsync(transaction);
                        //            await _transactionService.MarkCreditTransactionAsCompletedAsync(transaction: transaction);

                        //            await _transactionService.InsertCommissionAsync(new Commission()
                        //            {
                        //                Amount = commissionOnReturnAmount,
                        //                TransactionId = transaction.Id,
                        //            });

                        //            //get the return amount and invested balance to the current balance 
                        //            var updatedCustomer = await _customerService.GetCustomerByIdAsync(customer.Id);
                        //            updatedCustomer.InvestedAmount += updatedCustomer.CurrentBalance;
                        //            updatedCustomer.CurrentBalance = default;
                        //            updatedCustomer.LastReturnDate = dateTime;
                        //            await _customerService.UpdateCustomerAsync(updatedCustomer);
                        //        }
                        //        break;
                        //    case PaymentTypeEnum.PayOut:
                        //        {
                        //            var withdrawalMethod = await _withdrawService.GetWithdrawalMethodByIdAsync(customer.DefaultWithdrawalMethodId);
                        //            if (!(withdrawalMethod?.IsEnabled) ?? false)
                        //                break;

                        //            withdrawalMethod.IsRequested = true;
                        //            withdrawalMethod = await _withdrawService.CreateDuplicateWithdrawalMethodForCustomerAsync(withdrawalMethod, customer);

                        //            var transaction = new Transaction()
                        //            {
                        //                CustomerId = customer.Id,
                        //                TransactionAmount = -returnOnInvestedAmount,
                        //                TransactionType = TransactionType.Debit,
                        //                Status = Status.Pending,
                        //                TransactionNote = "Withraw: " + await _localizationService.GetLocalizedEnumAsync(withdrawalMethod.Type),
                        //                WithdrawalMethodId = withdrawalMethod.Id,
                        //                ReturnTransactionId = returnTransaction.Id
                        //            };
                        //            await _transactionService.InsertTransactionAsync(transaction);

                        //            customer.LastReturnDate = dateTime;
                        //            await _customerService.UpdateCustomerAsync(customer);
                        //        }
                        //        break;
                        //    default:
                        //        _logger.Error($"No payment type found in customer (ID: {customer.Id}) record.");
                        //        break;
                        //}
                    }
                    catch (Exception ex)
                    {
                        _logger.Error($"Error on returning amounts: {ex.Message}, Customer ID: {customer.Id}", ex);
                    }
                }

                await _transactionService.GenerateReturnAmountAsync();
            }
        }

        #endregion
    }
}
