using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Azure;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office.CustomUI;
using DocumentFormat.OpenXml.Office2010.Excel;
using HarfBuzzSharp;
using MaxMind.GeoIP2.Responses;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Transaction;
using Nop.Core.Infrastructure;
using Nop.Data;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;

namespace Nop.Services.Transactions
{
    public partial class TransactionService : ITransactionService
    {
        #region Fields

        protected readonly IRepository<Transaction> _transactionRepository;
        protected readonly ICustomerService _customerService;
        protected readonly IRepository<Commission> _commissionRepository;
        protected readonly TransactionSettings _transactionSettings;
        protected readonly ILocalizationService _localizationService;
        protected readonly ILogger _logger;
        protected readonly ICustomerActivityService _customerActivityService;
        protected readonly IRepository<WithdrawalMethod> _withdrawalMethodRepository;
        protected readonly IRepository<ReturnTransaction> _returnTransactionRepository;
        protected readonly IOrderService _orderService;
        protected readonly CustomerSettings _customerSettings;
        protected readonly IPaymentPluginManager _paymentPluginManager;
        protected readonly IWorkContext _workContext;
        protected readonly IWithdrawService _withdrawService;
        protected readonly ISettingService _settingService;
        protected readonly IWorkflowMessageService _workflowMessageService;

        #endregion

        #region Ctor

        public TransactionService(IRepository<Transaction> transactionRepository,
            ICustomerService customerService,
            IRepository<Commission> commissionRepository,
            TransactionSettings transactionSettings,
            ILocalizationService localizationService,
            ILogger logger,
            ICustomerActivityService customerActivityService,
            IRepository<WithdrawalMethod> withdrawalMethodRepository,
            IRepository<ReturnTransaction> returnTransactionRepository,
            IOrderService orderService,
            CustomerSettings customerSettings,
            IPaymentPluginManager paymentPluginManager,
            IWorkContext workContext,
            IWithdrawService withdrawService,
            ISettingService settingService)
        {
            _transactionRepository = transactionRepository;
            _customerService = customerService;
            _commissionRepository = commissionRepository;
            _transactionSettings = transactionSettings;
            _localizationService = localizationService;
            _logger = logger;
            _customerActivityService = customerActivityService;
            _withdrawalMethodRepository = withdrawalMethodRepository;
            _returnTransactionRepository = returnTransactionRepository;
            _orderService = orderService;
            _customerSettings = customerSettings;
            _paymentPluginManager = paymentPluginManager;
            _workContext = workContext;
            _withdrawService = withdrawService;
            _settingService = settingService;
            _workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
        }

        #endregion

        #region Methods

        #region Utilities

        private async Task UpdateCustomerWalletAsync(Transaction transaction, Customer customer = null)
        {
            customer ??= await _customerService.GetCustomerByIdAsync(transaction.CustomerId);

            switch (transaction.TransactionType)
            {
                case TransactionType.Debit:
                    {
                        //remove transaction amount from current balance at all conditions, either if current balance goes in minus then remove it from invested amount,
                        //transaction amount should not be exceeded from the total balance in wallet

                        if (transaction.TransactionAmount > (customer.CurrentBalance + customer.InvestedAmount))
                        {
                            throw new Exception(await _localizationService.GetResourceAsync("Customer.Withdraw.Transaction.NotEnoughBalance"));
                        }

                        if ((Math.Abs(transaction.TransactionAmount) - customer.CurrentBalance) > 0)
                        {
                            //in debit transaction amount will be in minus (-2000)
                            customer.InvestedAmount -= (Math.Abs(transaction.TransactionAmount) - customer.CurrentBalance);

                            //when credit any amount, customer's current balance will update and added with the credited amount
                            await InsertTransactionAsync(new Transaction()
                            {
                                CustomerId = transaction.CustomerId,
                                TransactionAmount = Math.Abs(transaction.TransactionAmount) - customer.CurrentBalance,
                                TransactionNote = "Deducted from invested balance",
                                TransactionType = TransactionType.Credit,
                                Status = Status.Completed
                            });
                        }

                        customer.CurrentBalance += transaction.TransactionAmount;

                        await _customerService.UpdateCustomerAsync(customer);
                    }
                    break;
                case TransactionType.Credit:
                    {
                        if (transaction.Status.Equals(Status.Completed))
                        {
                            customer.CurrentBalance += transaction.TransactionAmount;
                            await _customerService.UpdateCustomerAsync(customer);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Transaction

        public virtual async Task<IPagedList<Transaction>> GetAllTransactionsAsync(DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int customerId = default,
            int? orderId = default,
            decimal? balance = default,
            int transactionTypeId = default,
            int statusId = default,
            int orderBy = 1,
            string transactionNote = default,
            int pageSize = int.MaxValue,
            int pageIndex = default)
        {
            return await _transactionRepository.GetAllPagedAsync(query =>
            {
                if (startOnUtc.HasValue)
                    query = query.Where(x => x.CreatedOnUtc >= startOnUtc.Value);

                if (endOnUtc.HasValue)
                    query = query.Where(x => x.CreatedOnUtc <= endOnUtc.Value);

                if (!customerId.Equals(default(int)))
                    query = query.Where(x => x.CustomerId.Equals(customerId));

                if (balance.HasValue)
                    query = query.Where(x => x.Balance.Equals(balance));

                if (!transactionTypeId.Equals(default(int)))
                    query = query.Where(x => x.TransactionTypeId.Equals(transactionTypeId));

                if (!statusId.Equals(default(int)))
                    query = query.Where(x => x.StatusId.Equals(statusId));

                if (orderId.HasValue)
                    query = query.Where(x => x.OrderId.Equals(orderId.Value));

                if (orderBy.Equals(1))
                    query = query.OrderBy(x => x.CreatedOnUtc);
                else if (orderBy.Equals(2))
                    query = query.OrderByDescending(x => x.CreatedOnUtc);

                if (!string.IsNullOrEmpty(transactionNote))
                    query = query.Where(x => x.TransactionNote.Contains(transactionNote));

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _transactionRepository.GetByIdAsync(id);
        }

        public virtual async Task<Transaction> GetTransactionByOrderIdAsync(int orderId)
        {
            return await _transactionRepository.Table.FirstOrDefaultAsync(x => x.OrderId.GetValueOrDefault().Equals(orderId));
        }

        public virtual async Task<Transaction> GetTransactionByReturnTransactionAsync(ReturnTransaction returnTransaction)
        {
            return await _transactionRepository.GetByIdAsync(returnTransaction.TransactionId);
        }

        public virtual async Task<IList<Transaction>> GetTransactionsByIdsAsync(int[] ids)
        {
            return await _transactionRepository.GetByIdsAsync(ids);
        }

        public virtual async Task InsertTransactionAsync(Transaction transaction, bool sendEmail = true)
        {
            if (transaction is not null)
            {
                var customer = await _customerService.GetCustomerByIdAsync(transaction.CustomerId);
                if (!await _customerService.IsRegisteredAsync(customer) || !customer.Verified)
                    throw new UnauthorizedAccessException();

                transaction.CreatedOnUtc = _transactionSettings.SimulatedDateTime ?? DateTime.UtcNow;
                transaction.UpdatedOnUtc = transaction.CreatedOnUtc;
            }

            await _transactionRepository.InsertAsync(transaction);

            if (sendEmail)
            {
                await this.SendEmailOnTransactionAsync(transaction);
            }
        }

        public virtual async Task UpdateTransactionAsync(Transaction transaction)
        {
            if (transaction is not null)
                transaction.UpdatedOnUtc = _transactionSettings.SimulatedDateTime ?? DateTime.UtcNow;

            await _transactionRepository.UpdateAsync(transaction);
        }

        public virtual async Task DeleteTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.DeleteAsync(transaction);
        }

        public virtual async Task MarkCreditTransactionAsCompletedAsync(Order order, bool sendEmail = true)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var transaction = await this.GetTransactionByOrderIdAsync(orderId: order.Id);

            if (transaction is not null &&
                transaction.Status.Equals(Status.Pending) &&
                transaction.TransactionType.Equals(TransactionType.Credit) &&
                order.PaymentStatus.Equals(PaymentStatus.Paid) &&
                order.OrderStatus.Equals(OrderStatus.Complete))
            {
                await MarkCreditTransactionAsCompletedAsync(transaction: transaction, sendEmail: sendEmail);
            }
        }

        public virtual async Task MarkCreditTransactionAsCompletedAsync(Transaction transaction, bool sendEmail = true)
        {
            var customer = await _customerService.GetCustomerByIdAsync(transaction.CustomerId);
            if (customer is not null)
            {
                if (_transactionSettings.InvestmentDateStart <= DateTime.Now.Day &&
                    DateTime.Now.Day <= _transactionSettings.InvestmentDateEnd)
                {
                    customer.InvestedAmount += transaction.TransactionAmount;
                }
                else
                {
                    customer.CurrentBalance += transaction.TransactionAmount;
                }
                transaction.Status = Status.Completed;

                await this.UpdateTransactionAsync(transaction);
                await _customerService.UpdateCustomerAsync(customer);

                await _customerActivityService.InsertActivityAsync(customer, "TransactionLog",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.Customer.Invest.Transaction.Successfull"), transaction.TransactionAmount), transaction);

                if (sendEmail)
                {
                    await this.SendEmailOnTransactionAsync(transaction);
                }
            }
        }

        public virtual async Task GenerateReturnAmountAsync()
        {
            var customers = await (await _customerService.GetAllCustomersAsync(dontInvestAmount: false))
                .WhereAwait(async c => await _customerService.IsRegisteredAsync(c)).ToListAsync();

            foreach (var customer in customers)
            {
                //if (_transactionSettings.InvestmentDateStart <= DateTime.Now.Day &&
                //    DateTime.Now.Day <= _transactionSettings.InvestmentDateEnd)
                //{
                //    customer.ReturnAmountPercentagePerday = default;
                //}
                //else
                //{
                var apiResponse = await this.GetReturnPercentageOfCustomerTransactionsAsync(customerCommission: customer.CommissionToHouse,
                getCurrentMonthPercentage: true);
                customer.ReturnAmountPercentagePerday = apiResponse.investorInterestPercentage;
                //}
                await _customerService.UpdateCustomerAsync(customer);
            }
        }

        public virtual async Task<Transaction> MakeTransactionAfterOrderCreation(Order order, Customer customer)
        {
            if (order is null ||
                customer is null)
                return null;

            //if transaction is made then process the transaction and redirect to the invest page
            var orderItems = await _orderService.GetOrderItemsAsync(orderId: order.Id);
            if (orderItems.Any(x => x.ProductId.Equals(_customerSettings.TransactionProductId)))
            {
                try
                {
                    //payment method
                    var paymentMethod = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
                    var paymentMethodStr = paymentMethod != null
                        ? await _localizationService.GetLocalizedFriendlyNameAsync(paymentMethod, (await _workContext.GetWorkingLanguageAsync()).Id)
                        : order.PaymentMethodSystemName;

                    var transaction = new Transaction()
                    {
                        CustomerId = customer.Id,
                        TransactionAmount = order.OrderSubtotalExclTax,
                        TransactionType = TransactionType.Credit,
                        Status = Status.Pending,
                        TransactionNote = "Deposit: " + paymentMethodStr,
                        OrderId = order.Id,
                    };
                    await this.InsertTransactionAsync(transaction);

                    return transaction;
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync($"Error on invest: {ex.Message}", ex, customer);
                }
            }

            //if comes up here means order is not process with the transaction product
            return null;
        }

        public virtual async Task<Transaction> MakeDebitTransaction(decimal transactionAmount, WithdrawalMethod withdrawalMethod, Customer customer)
        {
            if (withdrawalMethod is null ||
               customer is null)
                return null;

            try
            {
                withdrawalMethod.IsRequested = true;
                withdrawalMethod = await _withdrawService.CreateDuplicateWithdrawalMethodForCustomerAsync(withdrawalMethod, customer);

                var transaction = new Transaction()
                {
                    CustomerId = customer.Id,
                    TransactionAmount = -transactionAmount,
                    TransactionType = TransactionType.Debit,
                    Status = Status.Pending,
                    TransactionNote = "Withraw: " + await _localizationService.GetLocalizedEnumAsync(withdrawalMethod.Type),
                    WithdrawalMethodId = withdrawalMethod.Id,
                };
                await this.InsertTransactionAsync(transaction);

                return transaction;
            }
            catch (Exception ex)
            {
                await _logger.ErrorAsync($"Error on withdrawal: {ex.Message}", ex, customer);
            }

            //if comes up here means order is not process with the transaction product
            return null;
        }

        public virtual async Task RollBackOrderTransactionAsync(Order order)
        {
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var transaction = await this.GetTransactionByOrderIdAsync(orderId: order.Id);

            if (transaction is not null &&
                transaction.Status.Equals(Status.Completed) &&
                transaction.TransactionType.Equals(TransactionType.Credit) &&
                order.PaymentStatus.Equals(PaymentStatus.Pending) &&
                order.OrderStatus.Equals(OrderStatus.Pending))
            {
                var customer = await _customerService.GetCustomerByIdAsync(transaction.CustomerId);
                if (customer is not null)
                {
                    customer.CurrentBalance -= transaction.TransactionAmount;
                    if (customer.CurrentBalance < 0)
                    {
                        customer.InvestedAmount -= Math.Abs(customer.CurrentBalance);
                        customer.CurrentBalance = default;
                    }
                    transaction.Status = Status.Pending;

                    await _customerService.UpdateCustomerAsync(customer);
                    await this.UpdateTransactionAsync(transaction);
                }
            }
        }

        #endregion

        #region Vault

        public virtual async Task<decimal> GetCustomerLastInvestedBalanceAsync(int customerId,
            DateTime? dateTime = default)
        {
            if (customerId.Equals(default(int)))
                return decimal.Zero;

            return (await GetAllTransactionDuringInvestmentPeriodByCustomerAsync(customerId: customerId,
                dateTime: dateTime)).LastOrDefault()?.UpdateBalance ?? decimal.Zero;
        }

        public virtual async Task<IList<Transaction>> GetAllTransactionDuringInvestmentPeriodByCustomerAsync(int customerId,
            DateTime? dateTime = default)
        {
            return await _transactionRepository.GetAllAsync(query =>
            {
                query = query.Where(x => x.CustomerId.Equals(customerId) &&
                    !x.StatusId.Equals((int)Status.Removed));

                if (dateTime.HasValue)
                    query = query.Where(x => x.CreatedOnUtc <= dateTime.Value);
                else
                    query = query.Where(x => x.CreatedOnUtc <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, _transactionSettings.InvestmentDateEnd, 23, 59, 59));

                return query;
            });
        }

        public virtual async Task<decimal> GetTotalAmountOfPendingDeposits()
        {
            //List<int> osIds = new List<int> { (int)OrderStatus.Pending };
            List<int> psIds = new List<int> { (int)PaymentStatus.Pending };
            IPagedList<Order> orders = await _orderService.SearchOrdersAsync(psIds: psIds);

            decimal total = orders
                //.Where(x => x.OrderStatus == OrderStatus.Pending)
                .Sum(x => x.OrderTotal);

            return total;
        }

        public virtual decimal GetLiquidityPoolValue()
        {
            return _transactionSettings.LiquidityPoolValue;
        }

        public virtual async Task<int> GetNumberOfPendingOrders()
        {
            //List<int> osIds = new List<int> { (int)OrderStatus.Pending };
            List<int> psIds = new List<int> { (int)PaymentStatus.Pending };
            IPagedList<Order> orders = await _orderService.SearchOrdersAsync(psIds: psIds);
            return orders.Count;
        }

        public virtual async Task CancelAllPendingOrders()
        {
            int defaultRetentionDays = 30;
            int retentionDays = _transactionSettings.OrderRetentionDays > 0 ?
                _transactionSettings.OrderRetentionDays :
                defaultRetentionDays;
            DateTime lastRetentionDate = DateTime.Now.AddDays(-retentionDays);
            //List<int> osIds = new List<int> { (int)OrderStatus.Pending };
            List<int> psIds = new List<int> { (int)PaymentStatus.Pending };

            IPagedList<Order> orders = await _orderService.SearchOrdersAsync(psIds: psIds, createdToUtc: lastRetentionDate);

            // Iterate over each order and update its status
            foreach (Order order in orders)
            {
                if (order.PaymentStatus == PaymentStatus.Pending)
                {
                    order.PaymentStatus = PaymentStatus.Voided;
                    order.OrderStatus = OrderStatus.Cancelled;
                }

                if (order.PaymentStatus == PaymentStatus.Refunded)
                {
                    order.OrderStatus = OrderStatus.Cancelled;
                }
                await _orderService.UpdateOrderAsync(order); // Update one at a time
            }
        }

        #endregion

        #region API

        public virtual async Task<CommissionApiResponse> GetReturnPercentageOfCustomerTransactionsAsync(decimal customerCommission, bool getCurrentMonthPercentage = false)
        {
            decimal defaultCommissionPercentage = _transactionSettings.DefaultCommissionPercentage != 0 ? _transactionSettings.DefaultCommissionPercentage : 20;
            decimal commissionPercent = customerCommission == 0 ? defaultCommissionPercentage : customerCommission;

            //since this method will be called from task that will run on the 1st day of month
            DateTime currentDateTime = DateTime.Now,
                previousDateTime = currentDateTime.AddMonths(-1),
                startDate = new DateTime(previousDateTime.Year, previousDateTime.Month, 1),
                endDate = new DateTime(previousDateTime.Year, previousDateTime.Month, new DateTime(currentDateTime.Year, currentDateTime.Month, 1).AddDays(-1).Day);

            //if request is coming to get the return of uptill now
            if (getCurrentMonthPercentage)
            {
                startDate = new DateTime(currentDateTime.Year, currentDateTime.Month, 1);
                endDate = currentDateTime;

                //if (_transactionSettings.InvestmentDateStart <= startDate.Day &&
                //    endDate.Day <= _transactionSettings.InvestmentDateEnd)
                //    return new CommissionApiResponse();
            }

            var client = new HttpClient();
            var apiRootUrl = _transactionSettings.ApiRootUrl ?? "https://interest-generator-api.azurewebsites.net";
            var request = new HttpRequestMessage(HttpMethod.Get, $"{apiRootUrl}/interest/?" +
                $"session={_transactionSettings.ApiSession}&" +
                $"poolId={_transactionSettings.ApiPoolId}&" +
                $"startDate={startDate:yyyy-MM-dd}&" +
                $"endDate={endDate:yyyy-MM-dd}&" +
                $"commissionPercent={commissionPercent}");
            var response = await client.SendAsync(request);
            var responseData = JsonConvert.DeserializeObject<CommissionApiResponse>(await response.Content.ReadAsStringAsync())
                ?? new CommissionApiResponse();
            responseData.IsSuccessStatusCode = response.IsSuccessStatusCode;

            if (!responseData.IsSuccessStatusCode || responseData.error || responseData == null)
            {
                await _logger.ErrorAsync(responseData.message);
            }

            return responseData;
        }

        public virtual async Task<decimal> GetCurrentLiquidityLimitAsync(bool includePendingDeposits = true)
        {
            var allCustomersBalance = await (await _customerService.GetAllCustomersAsync())
                .WhereAwait(async c => c.Active &&
                    c.Verified &&
                    await _customerService.IsRegisteredAsync(c))
                .SumAsync(c => c.InvestedAmount + c.CurrentBalance);

            if (includePendingDeposits)
            {
                var totalPendingAmount = await GetTotalAmountOfPendingDeposits();
                allCustomersBalance += totalPendingAmount;
            }
            return (_transactionSettings.LiquidityPoolValue / 2) - allCustomersBalance;
        }

        public virtual async Task<LiquidityPoolBalanceApiResponse> UpdateLiquidityPoolBalanceAsync()
        {
            var client = new HttpClient();
            var apiRootUrl = _transactionSettings.ApiRootUrl ?? "https://interest-generator-api.azurewebsites.net";
            var request = new HttpRequestMessage(
                HttpMethod.Get,
                $"{apiRootUrl}/LiquidityPool/GetBalance?session={_transactionSettings.ApiSession}");
            var response = await client.SendAsync(request);
            var responseData = JsonConvert.DeserializeObject<LiquidityPoolBalanceApiResponse>(await response.Content.ReadAsStringAsync())
                ?? new LiquidityPoolBalanceApiResponse();
            responseData.IsSuccessStatusCode = response.IsSuccessStatusCode;

            if (!responseData.IsSuccessStatusCode || responseData.error || responseData == null)
            {
                await _logger.ErrorAsync(responseData.message);
            }

            //Update LiquidityPoolValue in the settings
            _transactionSettings.LiquidityPoolValue = Convert.ToDecimal(responseData.equity);
            await _settingService.SaveSettingAsync(_transactionSettings);

            return responseData;
        }

        #endregion

        #region Commission

        public virtual async Task<IPagedList<Commission>> GetAllCommissionsAsync(
            int customerId = default,
            DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int transactionId = default,
            int pageSize = int.MaxValue,
            int pageIndex = default)
        {
            return await _commissionRepository.GetAllPagedAsync(query =>
            {
                if (startOnUtc.HasValue)
                    query = query.Where(x => x.CreatedOnUtc >= startOnUtc.Value);

                if (endOnUtc.HasValue)
                    query = query.Where(x => x.CreatedOnUtc <= endOnUtc.Value);

                if (!transactionId.Equals(default(int)))
                    query = query.Where(x => x.TransactionId.Equals(transactionId));

                if (!customerId.Equals(default))
                    query = from q in query
                            join t in _transactionRepository.Table on q.TransactionId equals t.Id
                            where t.CustomerId == customerId
                            orderby q.CreatedOnUtc descending
                            select q;

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<IPagedList<Commission>> GetAllCommissionsGroupedAsync(
            int customerId = default,
            DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int transactionId = default,
            int pageSize = int.MaxValue,
            int pageIndex = default)
        {
            return await _commissionRepository.GetAllPagedAsync(query =>
            {
                if (startOnUtc.HasValue)
                    query = query.Where(x => x.CreatedOnUtc >= startOnUtc.Value);

                if (endOnUtc.HasValue)
                    query = query.Where(x => x.CreatedOnUtc <= endOnUtc.Value);

                if (!transactionId.Equals(default(int)))
                    query = query.Where(x => x.TransactionId.Equals(transactionId));

                query = from q in query
                        join t in _transactionRepository.Table on q.TransactionId equals t.Id
                        where t.CustomerId == customerId || customerId.Equals(default(int))
                        orderby q.CreatedOnUtc descending
                        group q by t.CustomerId into grouped
                        select new Commission()
                        {
                            CustomerId = grouped.Key,
                            Amount = grouped.Sum(q => q.Amount), // Assuming q.Amount is the property representing the paid amount
                        };

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<Commission> GetCommissionByIdAsync(int id)
        {
            return await _commissionRepository.GetByIdAsync(id,
                cache => cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<Commission>.ByIdCacheKey, id));
        }

        public virtual async Task InsertCommissionAsync(Commission commission)
        {
            if (commission is not null)
                commission.CreatedOnUtc = _transactionSettings.SimulatedDateTime ?? DateTime.UtcNow;

            await _commissionRepository.InsertAsync(commission);
        }

        #endregion

        #region Return Transaction

        public virtual async Task<IPagedList<ReturnTransaction>> GetAllReturnTransactionsAsync(DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int month = default,
            int year = default,
            int customerId = default,
            int pageSize = int.MaxValue,
            int pageIndex = default)
        {
            return await _returnTransactionRepository.GetAllPagedAsync(async query =>
            {
                if (startOnUtc.HasValue)
                    query = query.Where(x => x.ReturnDateOnUtc >= startOnUtc.Value);

                if (endOnUtc.HasValue)
                    query = query.Where(x => x.ReturnDateOnUtc <= endOnUtc.Value);

                if (!month.Equals(default))
                    query = query.Where(x => x.ReturnDateOnUtc.Month.Equals(month));
                
                if (!year.Equals(default))
                    query = query.Where(x => x.ReturnDateOnUtc.Year.Equals(year));

                if (!customerId.Equals(default))
                {
                    var transactions = await this.GetAllTransactionsAsync(customerId: customerId);
                    query = from rt in query
                            join t in transactions on rt.TransactionId equals t.Id
                            select rt;
                }

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<ReturnTransaction> GetReturnTransactionByIdAsync(int id)
        {
            return await _returnTransactionRepository.GetByIdAsync(id);
        }

        public virtual async Task<IList<ReturnTransaction>> GetReturnTransactionsByIdsAsync(int[] ids)
        {
            return await _returnTransactionRepository.GetByIdsAsync(ids);
        }

        public virtual async Task InsertReturnTransactionAsync(ReturnTransaction returnTransaction)
        {
            await _returnTransactionRepository.InsertAsync(returnTransaction);
        }

        public virtual async Task UpdateTransactionAsync(ReturnTransaction returnTransaction)
        {
            await _returnTransactionRepository.UpdateAsync(returnTransaction);
        }

        public virtual async Task DeleteTransactionAsync(ReturnTransaction returnTransaction)
        {
            await _returnTransactionRepository.DeleteAsync(returnTransaction);
        }

        #endregion

        #region Email

        public virtual async Task SendEmailOnTransactionAsync(Transaction transaction)
        {
            var language = await _workContext.GetWorkingLanguageAsync();
            switch (transaction.Status)
            {
                case Status.Completed:
                    {
                        switch (transaction.TransactionType)
                        {
                            case TransactionType.Debit:
                                {
                                    await _workflowMessageService.SendCompletedDebitTransactionAdminNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                    await _workflowMessageService.SendCompletedDebitTransactionCustomerNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                }
                                break;
                            case TransactionType.Credit:
                                {
                                    await _workflowMessageService.SendCompletedCreditTransactionAdminNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                    await _workflowMessageService.SendCompletedCreditTransactionCustomerNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                }
                                break;
                        }
                    }
                    break;
                case Status.Pending:
                    {
                        switch (transaction.TransactionType)
                        {
                            case TransactionType.Debit:
                                {
                                    await _workflowMessageService.SendPendingDebitTransactionAdminNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                    await _workflowMessageService.SendPendingDebitTransactionCustomerNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                }
                                break;
                            case TransactionType.Credit:
                                {
                                    await _workflowMessageService.SendPendingCreditTransactionAdminNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                    await _workflowMessageService.SendPendingCreditTransactionCustomerNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                }
                                break;
                        }
                    }
                    break;
                case Status.Removed:
                    {
                        switch (transaction.TransactionType)
                        {
                            case TransactionType.Debit:
                                {
                                    await _workflowMessageService.SendRemovedDebitTransactionAdminNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                    await _workflowMessageService.SendRemovedDebitTransactionCustomerNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                }
                                break;
                            case TransactionType.Credit:
                                {
                                    await _workflowMessageService.SendRemovedCreditTransactionAdminNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                    await _workflowMessageService.SendRemovedCreditTransactionCustomerNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                }
                                break;
                        }
                    }
                    break;
                case Status.Declined:
                    {
                        switch (transaction.TransactionType)
                        {
                            case TransactionType.Debit:
                                {
                                    await _workflowMessageService.SendDeclinedDebitTransactionAdminNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                    await _workflowMessageService.SendDeclinedDebitTransactionCustomerNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                }
                                break;
                            case TransactionType.Credit:
                                {
                                    await _workflowMessageService.SendDeclinedCreditTransactionAdminNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                    await _workflowMessageService.SendDeclinedCreditTransactionCustomerNotificationAsync(transaction: transaction,
                                        languageId: language.Id);
                                }
                                break;
                        }
                    }
                    break;
            }
        }

        #endregion

        #endregion
    }
}
