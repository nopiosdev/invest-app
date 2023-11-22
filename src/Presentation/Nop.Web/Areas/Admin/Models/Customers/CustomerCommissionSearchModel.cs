using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Customers
{
    /// <summary>
    /// Represents a customer search model
    /// </summary>
    public partial record CustomerCommissionSearchModel : BaseSearchModel
    {
        #region Ctor

        public CustomerCommissionSearchModel()
        {
            AvailableCustomer = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("Admin.Customers.CustomerCommission.List.CustomerRoles")]
        public int SelectCustomer { get; set; }

        public IList<SelectListItem> AvailableCustomer { get; set; }

        #endregion
    }
}