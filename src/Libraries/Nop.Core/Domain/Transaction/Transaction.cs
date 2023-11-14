using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Core.Domain.Transaction
{
    public partial class Transaction : BaseEntity
    {

        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }
        public int TransactionTypeId { get; set; }
        public string TransactionNote { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal UpdateBalance { get; set; }
        public int StatusId { get; set; }
        public int? OrderId { get; set; }

        
        public TransactionType TransactionType
        {
            get => (TransactionType)this.TransactionTypeId;
            set => this.TransactionTypeId = (int)value;
        }
        public Status Status
        {
            get => (Status)this.StatusId;
            set => this.StatusId = (int)value;
        }
    }
}
