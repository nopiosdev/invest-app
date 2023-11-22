using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Transaction;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
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

        #endregion

        #region Ctor

        public SendProfitTask(ILogger logger,
            TransactionSettings transactionSettings,
            ITransactionService transactionService,
            ICustomerService customerService,
            ISettingService settingService)
        {
            _logger = logger;
            _transactionSettings = transactionSettings;
            _transactionService = transactionService;
            _customerService = customerService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var nextDateTime = DateTime.Now.AddDays(1);
            if (nextDateTime.Day.Equals(_transactionSettings.InvestmentDateStart) ||
                _transactionSettings.SendProfitDevMode)
            {
                _transactionSettings.SendProfitDevMode = false;
                await _settingService.SaveSettingAsync(_transactionSettings, x => x.SendProfitDevMode);

                var customers = await (await _customerService.GetAllCustomersAsync(dontInvestAmount: false,
                    isInvested: true)).WhereAwait(async c => await _customerService.IsRegisteredAsync(c)).ToListAsync();

                foreach (var customer in customers)
                {
                    try
                    {
                        //come from external API 
                        var returnPercentage = await _transactionService.InvestCustomerTransactionsAsync(customerId: customer.Id);

                        var investedAmount = await _transactionService.GetCustomerLastInvestedBalanceAsync(customerId: customer.Id);
                        if (investedAmount <= 0)
                            continue;

                        var returnOnInvestedAmount = investedAmount * returnPercentage;
                        var commissionOnReturnAmount = returnOnInvestedAmount * (customer.CommissionToHouse / 100);

                        var transation = new Transaction()
                        {
                            CustomerId = customer.Id,
                            TransactionAmount = returnOnInvestedAmount,
                            TransactionType = TransactionType.Credit,
                            Status = Status.Completed,
                            TransactionNote = string.Empty
                        };
                        await _transactionService.InsertTransactionAsync(transation);

                        await _transactionService.InsertCommissionAsync(new Commission()
                        {
                            CustomerId = _transactionSettings.AdminCommissionAccount,
                            Amount = commissionOnReturnAmount,
                            TransactionId = transation.Id,
                        });

                        customer.IsInvested = false;
                        await _customerService.UpdateCustomerAsync(customer);
                    }
                    catch (Exception ex)
                    {
                        await _logger.ErrorAsync($"Error on returning amounts: {ex.Message}, Customer ID: {customer.Id}", ex);
                    }
                }

                //send investment amount to API
            }
        }

        #endregion
    }
}
