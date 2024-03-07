using System.Threading.Tasks;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Transactions
{
    public partial class UpdateLPValueTask : IScheduleTask
    {
        private readonly ITransactionService _transactionService;

        public UpdateLPValueTask(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task ExecuteAsync()
        {
            await _transactionService.UpdateLiquidityPoolBalanceAsync();
        }
    }
}
