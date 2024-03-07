using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Transaction;
using Nop.Core;
using Nop.Data;
using Nop.Core.Domain.Customers;

namespace Nop.Services.Transactions
{
    public partial class WithdrawService : IWithdrawService
    {
        #region Fields

        private readonly IRepository<WithdrawalMethod> _withdrawalMethodRepository;
        private readonly IRepository<WithdrawalMethodField> _withdrawalMethodFieldRepository;
        private readonly IRepository<CustomerWithdrawalMethod> _customerWithdrawalMethodRepository;

        #endregion

        #region Ctor

        public WithdrawService(IRepository<WithdrawalMethod> withdrawalMethodRepository,
            IRepository<WithdrawalMethodField> withdrawalMethodFieldRepository,
            IRepository<CustomerWithdrawalMethod> customerWithdrawalMethodRepository)
        {
            _withdrawalMethodRepository = withdrawalMethodRepository;
            _withdrawalMethodFieldRepository = withdrawalMethodFieldRepository;
            _customerWithdrawalMethodRepository = customerWithdrawalMethodRepository;
        }

        #endregion

        #region Methods

        #region WithdrawalMethod

        public virtual async Task<IPagedList<WithdrawalMethod>> GetAllWithdrawalMethodAsync(int typeId = default,
            string name = default,
            bool? isEnabled = default,
            bool isRequested = default,
            int customerId = default,
            int pageIndex = default,
            int pageSize = int.MaxValue)
        {
            return await _withdrawalMethodRepository.GetAllPagedAsync(query =>
            {
                if (!typeId.Equals((int)default))
                    query = query.Where(x => x.TypeId.Equals(typeId));

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(x => x.Name.Contains(name));

                if (isEnabled.HasValue)
                    query = query.Where(x => x.IsEnabled.Equals(isEnabled.Value));

                if (!customerId.Equals((int)default))
                    query = (from wm in query
                             join wmf in _withdrawalMethodFieldRepository.Table on wm.Id equals wmf.WithdrawalMethodId
                             join cwm in _customerWithdrawalMethodRepository.Table on wmf.Id equals cwm.WithdrawalMethodFieldId
                             where cwm.CustomerId.Equals(customerId)
                             select wm).Distinct();

                //if isRequested is null then considered as false
                query = query.Where(x => x.IsRequested.Equals(isRequested));

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<WithdrawalMethod> GetWithdrawalMethodByIdAsync(int id)
        {
            return await _withdrawalMethodRepository.GetByIdAsync(id);
        }

        public virtual async Task InsertWithdrawalMethodAsync(WithdrawalMethod withdrawalMethod)
        {
            await _withdrawalMethodRepository.InsertAsync(withdrawalMethod);
        }

        public virtual async Task UpdateWithdrawalMethodAsync(WithdrawalMethod withdrawalMethod)
        {
            await _withdrawalMethodRepository.UpdateAsync(withdrawalMethod);
        }

        public virtual async Task DeleteWithdrawalMethodAsync(WithdrawalMethod withdrawalMethod)
        {
            await _withdrawalMethodRepository.DeleteAsync(withdrawalMethod);
        }

        public virtual async Task<WithdrawalMethod> CreateDuplicateWithdrawalMethodForCustomerAsync(WithdrawalMethod withdrawalMethod, Customer customer)
        {
            //create duplicate record of withdrawal method for the transaction table

            //get withdrawal method fields before creating new one
            var withdrawlMethodFields = await this.GetAllWithdrawalMethodFieldAsync(withdrawalMethodId: withdrawalMethod.Id,
                isEnabled: true);
            await this.InsertWithdrawalMethodAsync(withdrawalMethod);

            foreach (var withdrawlMethodField in withdrawlMethodFields)
            {
                //get all customer withdrawal method field data before creating a new field
                var customerWithdrawlMethodFields = await this.GetAllCustomerWithdrawalMethodAsync(customerId: customer.Id,
                    withdrawalMethodFieldId: withdrawlMethodField.Id);

                withdrawlMethodField.WithdrawalMethodId = withdrawalMethod.Id;
                await this.InsertWithdrawalMethodFieldAsync(withdrawlMethodField);

                foreach (var customerWithdrawlMethodField in customerWithdrawlMethodFields)
                {
                    customerWithdrawlMethodField.WithdrawalMethodFieldId = withdrawlMethodField.Id;
                    await this.InsertCustomerWithdrawalMethodAsync(customerWithdrawlMethodField);
                }
            }

            return withdrawalMethod;
        }

        #endregion

        #region WithdrawalMethodField

        public virtual async Task<IPagedList<WithdrawalMethodField>> GetAllWithdrawalMethodFieldAsync(int withdrawalMethodId = default,
            string fieldName = default,
            bool? isEnabled = default,
            int pageIndex = default,
            int pageSize = int.MaxValue)
        {
            return await _withdrawalMethodFieldRepository.GetAllPagedAsync(query =>
            {
                if (!withdrawalMethodId.Equals((int)default))
                    query = query.Where(x => x.WithdrawalMethodId.Equals(withdrawalMethodId));

                if (!string.IsNullOrEmpty(fieldName))
                    query = query.Where(x => x.FieldName.Equals(fieldName));

                if (isEnabled.HasValue)
                    query = query.Where(x => x.IsEnabled.Equals(isEnabled.Value));

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<WithdrawalMethodField> GetWithdrawalMethodFieldByIdAsync(int id)
        {
            return await _withdrawalMethodFieldRepository.GetByIdAsync(id);
        }

        public virtual async Task InsertWithdrawalMethodFieldAsync(WithdrawalMethodField withdrawalMethodField)
        {
            await _withdrawalMethodFieldRepository.InsertAsync(withdrawalMethodField);
        }

        public virtual async Task UpdateWithdrawalMethodFieldAsync(WithdrawalMethodField withdrawalMethodField)
        {
            await _withdrawalMethodFieldRepository.UpdateAsync(withdrawalMethodField);
        }

        public virtual async Task DeleteWithdrawalMethodFieldAsync(WithdrawalMethodField withdrawalMethodField)
        {
            await _withdrawalMethodFieldRepository.DeleteAsync(withdrawalMethodField);
        }

        #endregion

        #region CustomerWithdrawalMethod

        public virtual async Task<IPagedList<CustomerWithdrawalMethod>> GetAllCustomerWithdrawalMethodAsync(int customerId = default,
            int withdrawalMethodFieldId = default,
            string value = default,
            bool isRequested = default,
            int pageIndex = default,
            int pageSize = int.MaxValue)
        {
            return await _customerWithdrawalMethodRepository.GetAllPagedAsync(async query =>
            {
                var _withdrawalMethodFields = await this.GetAllWithdrawalMethodFieldAsync(isEnabled: true);
                var _withdrawalMethod = await this.GetAllWithdrawalMethodAsync(isEnabled: true,
                    isRequested: isRequested);

                query = from cwm in query
                        join wmf in _withdrawalMethodFields on cwm.WithdrawalMethodFieldId equals wmf.Id
                        join wm in _withdrawalMethod on wmf.WithdrawalMethodId equals wm.Id
                        select cwm;

                if (!customerId.Equals((int)default))
                    query = query.Where(x => x.CustomerId.Equals(customerId));

                if (!withdrawalMethodFieldId.Equals((int)default))
                    query = query.Where(x => x.WithdrawalMethodFieldId.Equals(withdrawalMethodFieldId));

                if (!string.IsNullOrEmpty(value))
                    query = query.Where(x => x.Value.Equals(value));

                return query;
            }, pageIndex, pageSize);
        }

        public virtual async Task<CustomerWithdrawalMethod> GetCustomerWithdrawalMethodByIdAsync(int id)
        {
            return await _customerWithdrawalMethodRepository.GetByIdAsync(id);
        }

        public virtual async Task InsertCustomerWithdrawalMethodAsync(CustomerWithdrawalMethod customerWithdrawalMethod)
        {
            await _customerWithdrawalMethodRepository.InsertAsync(customerWithdrawalMethod);
        }

        public virtual async Task UpdateCustomerWithdrawalMethodAsync(CustomerWithdrawalMethod customerWithdrawalMethod)
        {
            await _customerWithdrawalMethodRepository.UpdateAsync(customerWithdrawalMethod);
        }

        public virtual async Task DeleteCustomerWithdrawalMethodAsync(CustomerWithdrawalMethod customerWithdrawalMethod)
        {
            await _customerWithdrawalMethodRepository.DeleteAsync(customerWithdrawalMethod);
        }

        #endregion

        #endregion
    }
}
