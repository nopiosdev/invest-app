using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.WithdrawMethods
{
    public partial record WithdrawalMethodFieldModel : BaseNopEntityModel
    {
        public int WithdrawalMethodId { get; set; }
        public string FieldName { get; set; }
        public bool IsEnabled { get; set; }
    }
}
