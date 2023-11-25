﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2010.Excel;
using HarfBuzzSharp;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Transaction;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;

namespace Nop.Services.Transactions
{
    public partial class TransactionService : ITransactionService
    {
        #region Fields

        private readonly IRepository<Transaction> _transactionRepository;
        private readonly ICustomerService _customerService;
        private readonly IRepository<Commission> _commissionRepository;
        private readonly TransactionSettings _transactionSettings;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public TransactionService(IRepository<Transaction> transactionRepository,
            ICustomerService customerService,
            IRepository<Commission> commissionRepository,
            TransactionSettings transactionSettings,
            ILocalizationService localizationService)
        {
            _transactionRepository = transactionRepository;
            _customerService = customerService;
            _commissionRepository = commissionRepository;
            _transactionSettings = transactionSettings;
            _localizationService = localizationService;
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

                        customer.CurrentBalance += transaction.TransactionAmount;

                        if (customer.CurrentBalance < 0)
                        {
                            customer.InvestedAmount += customer.CurrentBalance;
                            customer.CurrentBalance = default;
                        }
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
            decimal? balance = default,
            int transactionTypeId = default,
            int statusId = default,
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

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<Transaction> GetTransactionByIdAsync(int id)
        {
            return await _transactionRepository.GetByIdAsync(id, cache =>
                cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<Transaction>.ByIdCacheKey, id));
        }

        public virtual async Task<IList<Transaction>> GetTransactionsByIdsAsync(int[] ids)
        {
            return await _transactionRepository.GetByIdsAsync(ids, cache =>
                cache.PrepareKeyForShortTermCache(NopEntityCacheDefaults<Transaction>.ByIdsCacheKey, ids));
        }

        public virtual async Task InsertTransactionAsync(Transaction transaction)
        {
            if (transaction is not null)
            {
                var customer = await _customerService.GetCustomerByIdAsync(transaction.CustomerId);
                if (!await _customerService.IsRegisteredAsync(customer) || !customer.Verified)
                    throw new UnauthorizedAccessException();

                await UpdateCustomerWalletAsync(transaction, customer);

                transaction.CreatedOnUtc = DateTime.UtcNow;
                transaction.UpdatedOnUtc = transaction.CreatedOnUtc;
            }

            await _transactionRepository.InsertAsync(transaction);
        }
        public virtual async Task UpdateTransactionAsync(Transaction transaction)
        {
            if (transaction is not null)
                transaction.UpdatedOnUtc = DateTime.UtcNow;

            await _transactionRepository.UpdateAsync(transaction);
        }
        public virtual async Task DeleteTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.DeleteAsync(transaction);
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

        #endregion

        #region API

        public virtual async Task<decimal> InvestCustomerTransactionsAsync(int customerId, decimal investAmount)
        {
            var random = new Random();
            return Convert.ToDecimal(random.Next(0, 10) / 100);
        }

        #endregion

        #region Commission

        public virtual async Task<IPagedList<Commission>> GetAllCommissionsAsync(DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int customerId = default,
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

                if (!customerId.Equals(default(int)))
                    query = from q in query
                            join t in _transactionRepository.Table on q.TransactionId equals t.Id
                            where t.CustomerId == customerId
                            select q;

                if (!transactionId.Equals(default(int)))
                    query = query.Where(x => x.TransactionId.Equals(transactionId));

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
                commission.CreatedOnUtc = DateTime.UtcNow;

            await _commissionRepository.InsertAsync(commission);
        }

        #endregion

        #endregion
    }
}
