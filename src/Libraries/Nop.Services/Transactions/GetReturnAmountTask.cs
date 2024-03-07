using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Transaction;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Transactions
{
    public partial class GetReturnAmountTask : IScheduleTask
    {
        #region Fields

        private readonly ITransactionService _transactionService;

        #endregion

        #region Ctor

        public GetReturnAmountTask(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {

            await _transactionService.GenerateReturnAmountAsync();
        }

        #endregion
    }
}
