using DocumentFormat.OpenXml.EMMA;
using LinqToDB;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Transaction;
using Nop.Services.Localization;
using Nop.Services.Transactions;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.WithdrawMethods;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial class WithdrawalMethodModelFactory : IWithdrawalMethodModelFactory
    {
        #region Fields

        private readonly IWithdrawService _withdrawService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public WithdrawalMethodModelFactory(IWithdrawService withdrawService,
            ILocalizationService localizationService)
        {
            _withdrawService = withdrawService;
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        public virtual async Task<WithdrawalMethodSearchModel> PrepareWithdrawMethodSearchModelAsync(WithdrawalMethodSearchModel searchModel)
        {
            if (searchModel is null)
                throw new ArgumentNullException(nameof(searchModel));

            foreach (WalletTypeEnum enumValues in Enum.GetValues(typeof(WalletTypeEnum)))
            {
                searchModel.AvailableWalletTypes.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedEnumAsync(enumValues),
                    Value = $"{(int)enumValues}",
                });
            }
            searchModel.AvailableWalletTypes.Insert(0, new SelectListItem() { Text = "-- Select", Value = "0" });

            searchModel.SetGridPageSize();

            return searchModel;
        }
        public virtual async Task<WithdrawalMethodListModel> PrepareWithdrawMethodListModelAsync(WithdrawalMethodSearchModel searchModel)
        {
            if (searchModel is null)
                throw new ArgumentNullException(nameof(searchModel));

            var withdrawalMethods = await _withdrawService.GetAllWithdrawalMethodAsync(typeId: searchModel.WalletTypeId,
                name: searchModel.Name,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = await new WithdrawalMethodListModel().PrepareToGridAsync(searchModel, withdrawalMethods, () =>
            {
                return withdrawalMethods.SelectAwait(async withdrawalMethod =>
                {
                    var withdrawalMethodModel = withdrawalMethod.ToModel<WithdrawalMethodModel>();
                    withdrawalMethodModel.TypeString = await _localizationService.GetLocalizedEnumAsync(withdrawalMethodModel.Type);

                    return withdrawalMethodModel;
                });
            });

            return model;
        }

        public virtual async Task<WithdrawalMethodModel> PrepareWithdrawalMethodModelAsync(WithdrawalMethodModel model, WithdrawalMethod entity)
        {
            if (entity is not null)
            {
                if (model is null)
                {
                    model = entity.ToModel<WithdrawalMethodModel>();

                }
                model.WithdrawalMethodFieldSearchModel.WithdrawalMethodId = entity.Id;
                model.WithdrawalMethodFieldSearchModel.SetGridPageSize();
            }
            else
            {
                model.IsEnabled = true;
                model.WithdrawalMethodFieldModel.IsEnabled = true;
            }

            foreach (WalletTypeEnum enumValues in Enum.GetValues(typeof(WalletTypeEnum)))
            {
                model.AvailableWalletTypes.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedEnumAsync(enumValues),
                    Value = $"{(int)enumValues}",
                });
            }

            return model;
        }

        public virtual WithdrawalMethodFieldSearchModel PrepareWithdrawalMethodFieldSearchModel(WithdrawalMethodFieldSearchModel searchModel)
        {
            if (searchModel is null)
                throw new ArgumentNullException(nameof(searchModel));

            searchModel.SetGridPageSize();

            return searchModel;
        }
        public virtual async Task<WithdrawalMethodFieldListModel> PrepareWithdrawalMethodFieldListModelAsync(WithdrawalMethodFieldSearchModel searchModel)
        {
            if (searchModel is null)
                throw new ArgumentNullException(nameof(searchModel));

            var withdrawalMethodFields = await _withdrawService.GetAllWithdrawalMethodFieldAsync(withdrawalMethodId: searchModel.WithdrawalMethodId,
                pageIndex: searchModel.Page - 1,
                pageSize: searchModel.PageSize);

            var model = new WithdrawalMethodFieldListModel().PrepareToGrid(searchModel, withdrawalMethodFields, () =>
            {
                return withdrawalMethodFields.Select(withdrawalMethodField =>
                {
                    var withdrawalMethodFieldModel = withdrawalMethodField.ToModel<WithdrawalMethodFieldModel>();
                    return withdrawalMethodFieldModel;
                });
            });

            return model;
        }

        #endregion
    }
}
