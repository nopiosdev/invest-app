using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Transaction;
using Nop.Data;

namespace Nop.Services.Transactions
{
    public partial class TransactionService : ITransactionService
    {
        #region Fields

        private readonly IRepository<Transaction> _transactionRepository;

        #endregion

        #region Ctor

        public TransactionService(IRepository<Transaction> transactionRepository)
        {
            _transactionRepository = transactionRepository;
        }

        #endregion

        #region Methods

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
        public virtual async Task InsertTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.InsertAsync(transaction);
        }
        public virtual async Task UpdateTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.UpdateAsync(transaction);
        }
        public virtual async Task DeleteTransactionAsync(Transaction transaction)
        {
            await _transactionRepository.DeleteAsync(transaction);
        }

        #endregion
    }
}
