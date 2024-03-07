using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    public partial record CustomerWIthdrawalMethodSearchModel : BaseSearchModel
    {
        public CustomerWIthdrawalMethodSearchModel()
        {
            this.SetGridPageSize();
        }

        public int CustomerId { get; set; }
    }
}
