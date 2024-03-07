using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Transaction;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Customer
{
    public class WithdrawModel
    {
        public WithdrawModel()
        {
            RecentTransactions = new List<TransactionModel>();
            AvailableWalletType = new List<SelectListItem>();
            WithdrawalMethodModel = new List<WithdrawalMethodCustomerInfoModel>();
        }

        public IList<TransactionModel> RecentTransactions { get; set; }
        public int WalletTypeId { get; set; }
        public IList<SelectListItem> AvailableWalletType { get; set; }
        public decimal TransactionAmount { get; set; }

        //withdraw method fields
        public string DigitalWalletAccountEmail { get; set; }
        public string DigitalWalletAccountPhoneNumber { get; set; }
        public string DigitalWalletAccountHandle { get; set; }
        public string DigitalWalletAccountHolderName { get; set; }
        public string DigitalWalletInstitutionName { get; set; }
        public string CryptoWalletAccountWalletAddress { get; set; }
        public string BankTransferAccountNumber { get; set; }
        public string BankTransferAccountHolderName { get; set; }
        public string BankTransferSwiftBic { get; set; }
        public string BankTransferInstitutionName { get; set; }
        public string BankTransferInstitutionAddress { get; set; }

        public IList<WithdrawalMethodCustomerInfoModel> WithdrawalMethodModel { get; set; }
        public int WithdrawalMethodId { get; set; }
        public int DefaultWithdrawalMethodId { get; set; }
        public string CurrencySymbol { get; set; }

    }

    public partial record WithdrawalMethodCustomerInfoModel : BaseNopEntityModel
    {
        public WithdrawalMethodCustomerInfoModel()
        {
            Fields = new List<WithdrawalMethodCustomerInfoModel>();
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public IList<WithdrawalMethodCustomerInfoModel> Fields { get; set; }
    }
}
