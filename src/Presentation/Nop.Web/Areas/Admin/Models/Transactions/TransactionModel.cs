using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Transaction;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Customer;

namespace Nop.Web.Areas.Admin.Models.Transactions
{
    public partial record TransactionModel : BaseNopEntityModel
    {
        public TransactionModel()
        {
            this.AvaialableStatus = new List<SelectListItem>();
            this.AvaialableTransactionType = new List<SelectListItem>();
            this.WithdrawalMethodModel = new WithdrawalMethodCustomerInfoModel();
            this.WithdrawalMethodCustomerInfoModels = new List<WithdrawalMethodCustomerInfoModel>();
        }
        public TransactionModel(int customerId)
        {
            this.AvaialableStatus = new List<SelectListItem>();
            this.AvaialableTransactionType = new List<SelectListItem>();
            this.WithdrawalMethodModel = new WithdrawalMethodCustomerInfoModel();
            this.WithdrawalMethodCustomerInfoModels = new List<WithdrawalMethodCustomerInfoModel>();

            this.CustomerId = customerId;
        }

        public DateTime CreateOnUtc { get; set; }
        public int CustomerId { get; set; }
        public decimal Balance { get; set; }
        public int TransactionTypeId { get; set; }
        public string TransactionNote { get; set; }
        public decimal TransactionAmount { get; set; }
        public string FormattedTransactionAmount { get; set; }
        public decimal UpdateBalance { get; set; }
        public int StatusId { get; set; }
        public IList<SelectListItem> AvaialableStatus { get; set; }
        public IList<SelectListItem> AvaialableTransactionType { get; set; }
        public string CustomerInfo { get; set; }
        public bool UserCanDelete { get; set; }
        public bool CanChangeDate { get; set; }

        public string TransactionTypeString { get; set; }
        public string StatusString { get; set; }

        public WithdrawalMethodCustomerInfoModel WithdrawalMethodModel { get; set; }

        public IList<WithdrawalMethodCustomerInfoModel> WithdrawalMethodCustomerInfoModels { get; set; }
        public int WithdrawalMethodId { get; set; }

        public TransactionType TransactionType
        {
            get => (TransactionType)this.TransactionTypeId;
            set => this.TransactionTypeId = (int)value;
        }
        public Status Status
        {
            get => (Status)this.StatusId;
            set => this.StatusId = (int)value;
        }
    }
}
