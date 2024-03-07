using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Transaction
{
    public partial class CustomerWithdrawalMethod : BaseEntity
    {
        public int CustomerId { get; set; }
        public int WithdrawalMethodFieldId { get; set; }
        public string Value { get; set; }
    }
}
