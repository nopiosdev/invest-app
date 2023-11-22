﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Nop.Core;
using Nop.Core.Domain.Transaction;
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

        #endregion

        #region Ctor

        public InvestAmountTask(ILogger logger,
            TransactionSettings transactionSettings,
            ITransactionService transactionService,
            ICustomerService customerService,
            ISettingService settingService)
        {
            _logger = logger;
            _transactionSettings = transactionSettings;
            _transactionService = transactionService;
            _customerService = customerService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public async Task ExecuteAsync()
        {
            var previousDateTime = DateTime.Now.AddDays(-1);
            if (previousDateTime.Day.Equals(_transactionSettings.InvestmentDateEnd) ||
                _transactionSettings.InvestAmountDevMode)
            {
                _transactionSettings.InvestAmountDevMode = false;
                await _settingService.SaveSettingAsync(_transactionSettings, x => x.InvestAmountDevMode);

                var customers = await (await _customerService.GetAllCustomersAsync(dontInvestAmount: false,
                    isInvested: false)).WhereAwait(async c => await _customerService.IsRegisteredAsync(c)).ToListAsync();
                decimal totalInvestAmount = default;

                foreach (var customer in customers)
                {
                    totalInvestAmount += await _transactionService.GetCustomerLastInvestedBalanceAsync(customerId: customer.Id);

                    if (totalInvestAmount <= 0)
                        continue;

                    customer.IsInvested = true;
                    await _customerService.UpdateCustomerAsync(customer);
                }

                //send investment amount to API
            }
        }

        #endregion
    }
}
