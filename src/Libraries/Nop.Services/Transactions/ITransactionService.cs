using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Transaction;

namespace Nop.Services.Transactions
{
    public partial interface ITransactionService
    {
        Task<IPagedList<Transaction>> GetAllTransactionsAsync(DateTime? startOnUtc = default,
            DateTime? endOnUtc = default,
            int customerId = default,
            decimal? balance = default,
            int transactionTypeId = default,
            int statusId = default,
            int pageSize = int.MaxValue,
            int pageIndex = default);

        Task<Transaction> GetTransactionByIdAsync(int id);
        Task InsertTransactionAsync(Transaction transaction);
        Task UpdateTransactionAsync(Transaction transaction);
        Task DeleteTransactionAsync(Transaction transaction);
    }
}
