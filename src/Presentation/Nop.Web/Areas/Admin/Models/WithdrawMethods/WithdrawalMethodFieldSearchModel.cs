using Nop.Web.Framework.Models;

namespace Nop.Web.Areas.Admin.Models.WithdrawMethods
{
    public partial record WithdrawalMethodFieldSearchModel:BaseSearchModel
    {
        public int WithdrawalMethodId { get; set; }
    }
}
