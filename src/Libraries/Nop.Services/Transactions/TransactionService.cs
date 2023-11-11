using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Transaction;

namespace Nop.Services.Transactions
{
    public partial class TransactionService : ITransactionService
    {
        public Task DeleteTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<Transaction>> GetAllTransactionsAsync(DateTime startOnUtc = default, DateTime endOnUtc = default, int customerId = 0, decimal balance = 0, int transactionTypeId = 0, int statusId = 0, int pageSize = int.MaxValue, int pageIndex = 0)
        {
            throw new NotImplementedException();
        }

        public Task<Transaction> GetTransactionByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task InsertTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateTransactionAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
