using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    public partial record CustomerCommissionModel : BaseNopEntityModel
    {
        #region Properties
        [NopResourceDisplayName("Admin.Customers.CustomerCommission.List.CreatedOn")]
        public DateTime CreatedOn { get; set; }
        [NopResourceDisplayName("Admin.Customers.CustomerCommission.List.PaidCustomer")]
        public string PaidCustomer { get; set; }
        [NopResourceDisplayName("Admin.Customers.CustomerCommission.List.PaidAmount")]
        public decimal PaidAmount { get; set; }

        #endregion
    }
}
