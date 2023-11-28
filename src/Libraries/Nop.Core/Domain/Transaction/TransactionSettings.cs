using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Configuration;

namespace Nop.Core.Domain.Transaction
{
    public partial class TransactionSettings : ISettings
    {
        public bool SendProfitDevMode { get; set; }
        public bool InvestAmountDevMode { get; set; }
        /// <summary>
        /// Date must be only day of the month like 1st Jan,2022 -> 1 would be the start date
        /// </summary>
        public int InvestmentDateStart { get; set; }

        /// <summary>
        /// Date must be only day of the month like 3rd Jan,2022 -> 3 would be the end date
        /// </summary>
        public int InvestmentDateEnd { get; set; }
        public int AdminCommissionAccount { get; set; }
        public string ApiSession { get; set; }
        public string ApiPoolId { get; set; }
    }
}
