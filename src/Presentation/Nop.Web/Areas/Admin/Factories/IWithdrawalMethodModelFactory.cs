using Nop.Core.Domain.Transaction;
using Nop.Web.Areas.Admin.Models.WithdrawMethods;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial interface IWithdrawalMethodModelFactory
    {
        Task<WithdrawalMethodSearchModel> PrepareWithdrawMethodSearchModelAsync(WithdrawalMethodSearchModel searchModel);
        Task<WithdrawalMethodListModel> PrepareWithdrawMethodListModelAsync(WithdrawalMethodSearchModel searchModel);
        Task<WithdrawalMethodModel> PrepareWithdrawalMethodModelAsync(WithdrawalMethodModel model, WithdrawalMethod entity);
        WithdrawalMethodFieldSearchModel PrepareWithdrawalMethodFieldSearchModel(WithdrawalMethodFieldSearchModel searchModel);
        Task<WithdrawalMethodFieldListModel> PrepareWithdrawalMethodFieldListModelAsync(WithdrawalMethodFieldSearchModel searchModel);
    }
}
