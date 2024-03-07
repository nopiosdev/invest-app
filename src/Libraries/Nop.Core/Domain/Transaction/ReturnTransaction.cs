using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Transaction
{
    public partial class ReturnTransaction : BaseEntity
    {
        public decimal ReturnAmount { get; set; }
        public decimal ReturnPercentage { get; set; }
        public DateTime ReturnDateOnUtc { get; set; }
        public int TransactionId { get; set; }
    }
}
