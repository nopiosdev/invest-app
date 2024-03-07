using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Transaction;
using Nop.Services.Authentication.External;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.ScheduleTasks;

namespace Nop.Services.Transactions
{
    public partial class InvestAmountTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly TransactionSettings _transactionSettings;
        private readonly ITransactionService _transactionService;
        private readonly ICustomerService _customerService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;

        #endregion

        #region Ctor

        public InvestAmountTask(ILogger logger,
            TransactionSettings transactionSettings,
            ITransactionService transactionService,
            ICustomerService customerService,
            ISettingService settingService,
            IWorkContext workContext)
        {
            _logger = logger;
            _transactionSettings = transactionSettings;
            _transactionService = transactionService;
            _customerService = customerService;
            _settingService = settingService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            //runs on the 6th day of the month that is the next day of the ending day (_transactionSettings.InvestmentDateEnd)
            var previousDateTime = DateTime.Now.AddDays(-1);
            if (previousDateTime.Day.Equals(_transactionSettings.InvestmentDateEnd) ||
                await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
            {
                var customers = await (await _customerService.GetAllCustomersAsync(dontInvestAmount: false)).WhereAwait(async c => await _customerService.IsRegisteredAsync(c)&&
                    //if any customer has not invested any amount then zero amount will not be invested.
                    c.CurrentBalance>0&&
                    //don't invest any customer if it is forced to ask to not invest his amount
                    !c.DontInvestAmount).ToListAsync();
                decimal totalInvestAmount = default;

                foreach (var customer in customers)
                {
                    totalInvestAmount += customer.CurrentBalance;

                    customer.InvestedAmount = customer.CurrentBalance;
                    customer.CurrentBalance = default;
                    await _customerService.UpdateCustomerAsync(customer);
                }

                await _transactionService.GenerateReturnAmountAsync();
            }
        }

        #endregion
    }
}
