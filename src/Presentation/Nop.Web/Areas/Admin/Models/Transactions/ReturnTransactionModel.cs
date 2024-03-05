using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Transactions
{
    public partial record ReturnTransactionModel : BaseNopEntityModel
    {
        public string CustomerFullName { get; set; }
        public string CustomerEmail { get; set; }
        public int CustomerId { get; set; }
        public string WithdrawalMethod { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal ReturnPercentage { get; set; }
        public DateTime ReturnDateOnUtc { get; set; }
        public int TransactionId { get; set; }
    }
}
