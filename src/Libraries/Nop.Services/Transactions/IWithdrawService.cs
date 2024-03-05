using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Transaction;

namespace Nop.Services.Transactions
{
    public partial interface IWithdrawService
    {
        #region Withdrawal Method

        Task<IPagedList<WithdrawalMethod>> GetAllWithdrawalMethodAsync(int typeId = default,
            string name = default,
            bool? isEnabled = default,
            bool isRequested = default,
            int customerId = default,
            int pageIndex = default,
            int pageSize = int.MaxValue);
        Task<WithdrawalMethod> GetWithdrawalMethodByIdAsync(int id);
        Task InsertWithdrawalMethodAsync(WithdrawalMethod withdrawalMethod);
        Task UpdateWithdrawalMethodAsync(WithdrawalMethod withdrawalMethod);
        Task DeleteWithdrawalMethodAsync(WithdrawalMethod withdrawalMethod);
        Task<WithdrawalMethod> CreateDuplicateWithdrawalMethodForCustomerAsync(WithdrawalMethod withdrawalMethod, Customer customer);

        #endregion

        #region Withdrawal Method Field

        Task<IPagedList<WithdrawalMethodField>> GetAllWithdrawalMethodFieldAsync(int withdrawalMethodId = default,
             string fieldName = default,
             bool? isEnabled = default,
             int pageIndex = default,
             int pageSize = int.MaxValue);
        Task<WithdrawalMethodField> GetWithdrawalMethodFieldByIdAsync(int id);
        Task InsertWithdrawalMethodFieldAsync(WithdrawalMethodField withdrawalMethodField);
        Task UpdateWithdrawalMethodFieldAsync(WithdrawalMethodField withdrawalMethodField);
        Task DeleteWithdrawalMethodFieldAsync(WithdrawalMethodField withdrawalMethodField);

        #endregion

        #region Customer Withdrawal Method

        Task<IPagedList<CustomerWithdrawalMethod>> GetAllCustomerWithdrawalMethodAsync(int customerId = default,
            int withdrawalMethodFieldId = default,
            string value = default,
            bool isRequested = default,
            int pageIndex = default,
            int pageSize = int.MaxValue);
        Task<CustomerWithdrawalMethod> GetCustomerWithdrawalMethodByIdAsync(int id);
        Task InsertCustomerWithdrawalMethodAsync(CustomerWithdrawalMethod customerWithdrawalMethod);
        Task UpdateCustomerWithdrawalMethodAsync(CustomerWithdrawalMethod customerWithdrawalMethod);
        Task DeleteCustomerWithdrawalMethodAsync(CustomerWithdrawalMethod customerWithdrawalMethod);

        #endregion
    }
}
