using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Areas.Admin.Models.Transactions
{
    public partial record TransactionSearchModel : BaseSearchModel
    {
        public TransactionSearchModel()
        {
            AvailableStatus = new List<SelectListItem>();
            AvailableTransactionType = new List<SelectListItem>();

            this.SetGridPageSize();
        }

        [NopResourceDisplayName("Admin.Customers.Transaction.Fields.CreatedOnFrom")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnFrom { get; set; }

        [NopResourceDisplayName("Admin.Customers.Transaction.Fields.CreatedOnTo")]
        [UIHint("DateNullable")]
        public DateTime? CreatedOnTo { get; set; }

        public int StatusId { get; set; }
        public IList<SelectListItem> AvailableStatus { get; set; }

        public int TransactionTypeId { get; set; }
        public IList<SelectListItem> AvailableTransactionType { get; set; }

        public int CustomerId { get; set; }
    }
}
