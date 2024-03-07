using System.Threading.Tasks;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Transactions
{
    public partial class OrdersRetentionTask : IScheduleTask
    {
        private readonly ITransactionService _transactionService;

        public OrdersRetentionTask(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }
        public async Task ExecuteAsync()
        {
            await _transactionService.CancelAllPendingOrders();
        }
    }
}
