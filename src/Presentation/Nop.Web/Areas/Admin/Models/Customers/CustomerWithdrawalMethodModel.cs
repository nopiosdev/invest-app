using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    public partial record CustomerWithdrawalMethodModel:BaseNopEntityModel
    {
        public int CustomerId { get; set; }
        public string WithdrawalMethodName { get; set; }
        public string WithdrawalMethodType { get; set; }
    }
}
