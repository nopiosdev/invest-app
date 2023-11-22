using Nop.Core;
using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    public partial record CustomerCommissionListModel : BasePagedListModel<CustomerCommissionModel>
    {
        public decimal TotalPaidAmount { get; set; }
    }
}
