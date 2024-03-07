using System.Threading.Tasks;
using Nop.Core.Domain.Transaction;
using Nop.Web.Areas.Admin.Models.Transactions;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial interface ITransactionModelFactory
    {
        Task<TransactionSearchModel> PrepareTransactionSearchModelAsync(TransactionSearchModel searchModel);
        Task<TransactionListModel> PrepareTransactionListModelAsync(TransactionSearchModel searchModel);
        Task<TransactionModel> PrepareTransactionModelAsync(TransactionModel model, Transaction transaction);
        
        Task<ReturnTransactionSearchModel> PrepareReturnTransactionSearchModelAsync(ReturnTransactionSearchModel searchModel);
        Task<ReturnTransactionListModel> PrepareReturnTransactionListModelAsync(ReturnTransactionSearchModel searchModel);
    }
}
