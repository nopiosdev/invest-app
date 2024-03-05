using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.WithdrawMethods
{
    public partial record WithdrawalMethodSearchModel : BaseSearchModel
    {
        public WithdrawalMethodSearchModel()
        {
            AvailableWalletTypes = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.WithdrawMethod.Name")]
        public string Name { get; set; }
        [NopResourceDisplayName("Admin.WithdrawMethod.WalletTypeId")]
        public int WalletTypeId { get; set; }
        public IList<SelectListItem> AvailableWalletTypes { get; set; }
    }
}
