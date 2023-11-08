using Nop.Core.Domain.Transaction;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Transactions
{
    public partial record TransactionModel : BaseNopEntityModel
    {
        public DateTime CreateOnUtc { get; set; }
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }
        public int TransactionTypeId { get; set; }
        public string TransactionNote { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal UpdateBalance { get; set; }
        public int StatusId { get; set; }
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
