using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Transaction;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.WithdrawMethods
{
    public partial record WithdrawalMethodModel : BaseNopEntityModel
    {
        public WithdrawalMethodModel()
        {
            AvailableWalletTypes = new List<SelectListItem>();
            WithdrawalMethodFieldSearchModel = new WithdrawalMethodFieldSearchModel();
            WithdrawalMethodFieldModel = new WithdrawalMethodFieldModel();
        }

        [NopResourceDisplayName("Admin.WithdrawalMethod.TypeId")]
        public int TypeId { get; set; }
        public IList<SelectListItem> AvailableWalletTypes { get; set; }

        [NopResourceDisplayName("Admin.WithdrawalMethod.TypeId")]
        public string TypeString { get; set; }
        public WalletTypeEnum Type
        {
            get => (WalletTypeEnum)this.TypeId;
            set { this.TypeId = (int)value; }
        }

        [NopResourceDisplayName("Admin.WithdrawalMethod.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.WithdrawalMethod.IsEnabled")]
        public bool IsEnabled { get; set; }
        public WithdrawalMethodFieldSearchModel WithdrawalMethodFieldSearchModel { get; set; }
        public WithdrawalMethodFieldModel WithdrawalMethodFieldModel { get; set; }
    }
}
