using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Transactions
{
    public partial record ReturnTransactionSearchModel : BaseSearchModel
    {
        public ReturnTransactionSearchModel()
        {
            AvailableMonths = new List<SelectListItem>();
            AvailableYears = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.ReturnTransactionSearchModel.Field.Month")]
        public int Month { get; set; }
        public IList<SelectListItem> AvailableMonths { get; set; }

        [NopResourceDisplayName("Admin.ReturnTransactionSearchModel.Field.Year")]
        public int Year { get; set; }
        public IList<SelectListItem> AvailableYears { get; set; }
    }
}
