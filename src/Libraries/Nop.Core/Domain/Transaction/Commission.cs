using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Transaction
{
    public partial class Commission : BaseEntity
    {
        public decimal Amount { get; set; }
        public int TransactionId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        
        [NotMapped]
        public int CustomerId { get; set; }

        public decimal Percentage { get; set; }
    }
}
