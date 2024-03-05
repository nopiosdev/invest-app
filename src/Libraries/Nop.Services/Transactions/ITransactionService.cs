using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Transaction;

namespace Nop.Services.Transactions
{
    public partial interface ITransactionService
    {
        Task<IPagedList<Transaction>> GetAllTransactionsAsync(DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int customerId = default,
            int? orderId = default,
            decimal? balance = default,
            int transactionTypeId = default,
            int statusId = default,
            int orderBy = 1,
            string transactionNote = default,
            int pageSize = int.MaxValue,
            int pageIndex = default);

        Task<Transaction> GetTransactionByIdAsync(int id);

        Task<Transaction> GetTransactionByOrderIdAsync(int orderId);

        Task<Transaction> GetTransactionByReturnTransactionAsync(ReturnTransaction returnTransaction);

        Task<IList<Transaction>> GetTransactionsByIdsAsync(int[] ids);

        Task InsertTransactionAsync(Transaction transaction, bool sendEmail = true);

        Task UpdateTransactionAsync(Transaction transaction);

        Task DeleteTransactionAsync(Transaction transaction);

        Task MarkDepositTransactionAsCompletedAsync(Order order, bool sendEmail = true);

        Task MarkDepositTransactionAsCompletedAsync(Transaction transaction, bool sendEmail = true);

        Task GenerateReturnAmountAsync();
        /// <summary>
        /// Return Transaction entity that says success
        /// </summary>
        /// <param name="order">Transaction credit order</param>
        /// <returns></returns>
        Task<Transaction> MakeTransactionAfterOrderCreation(Order order, Customer customer);

        Task<Transaction> MakeDebitTransaction(decimal transactionAmount, WithdrawalMethod withdrawalMethod, Customer customer);

        Task RollBackOrderTransactionAsync(Order order);

        #region Vault

        Task<decimal> GetCustomerLastInvestedBalanceAsync(int customerId,
            DateTime? dateTime = default);

        Task<IList<Transaction>> GetAllTransactionDuringInvestmentPeriodByCustomerAsync(int customerId,
            DateTime? dateTime = default);

        Task<decimal> GetTotalAmountOfPendingDeposits();

        Task<int> GetNumberOfPendingOrders();

        decimal GetLiquidityPoolValue();

        Task CancelAllPendingOrders();
        #endregion

        #region API

        Task<CommissionApiResponse> GetReturnPercentageOfCustomerTransactionsAsync(decimal customerCommission, bool getCurrentPercentage = false);

        Task<decimal> GetCurrentLiquidityLimitAsync(bool includePendingDeposits = true);

        Task<LiquidityPoolBalanceApiResponse> UpdateLiquidityPoolBalanceAsync();

        #endregion

        #region Commission

        Task<IPagedList<Commission>> GetAllCommissionsAsync(
            int customerId = default,
            DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int transactionId = default,
            int pageSize = int.MaxValue,
            int pageIndex = default);

        Task<IPagedList<Commission>> GetAllCommissionsGroupedAsync(
            int customerId = default,
            DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int transactionId = default,
            int pageSize = int.MaxValue,
            int pageIndex = default);

        Task<Commission> GetCommissionByIdAsync(int id);
        Task InsertCommissionAsync(Commission commission);

        #endregion

        #region Return Transaction

        Task<IPagedList<ReturnTransaction>> GetAllReturnTransactionsAsync(DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int month = default,
            int year = default,
            int customerId = default,
            int pageSize = int.MaxValue,
            int pageIndex = default);

        Task<ReturnTransaction> GetReturnTransactionByIdAsync(int id);

        Task<IList<ReturnTransaction>> GetReturnTransactionsByIdsAsync(int[] ids);

        Task InsertReturnTransactionAsync(ReturnTransaction returnTransaction);

        Task UpdateTransactionAsync(ReturnTransaction returnTransaction);

        Task DeleteTransactionAsync(ReturnTransaction returnTransaction);

        #endregion

        #region Email

        Task SendEmailOnTransactionAsync(Transaction transaction);

        #endregion
    }
}
