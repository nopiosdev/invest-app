﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Transaction
{
    public partial class CommissionApiResponse: ApiBaseResponse
    {
        public decimal totalGeneratedPercentage { get; set; }
        public decimal investorInterestPercentage { get; set; }
        public string title { get; set; }
    }
}
