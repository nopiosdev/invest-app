using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Humanizer;

namespace Nop.Core.Domain.Transaction
{
    public partial class WithdrawalMethodField : BaseEntity
    {
        public int WithdrawalMethodId { get; set; }
        public string FieldName { get; set; }
        public bool IsEnabled { get; set; }
    }
}
