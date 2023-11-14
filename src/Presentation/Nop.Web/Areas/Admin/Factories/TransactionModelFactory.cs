using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Transaction;
using Nop.Services.Localization;
using Nop.Services.Transactions;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Transactions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Models.Extensions;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial class TransactionModelFactory : ITransactionModelFactory
    {
        #region Fields

        private readonly ITransactionService _transactionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor



        #endregion

        #region Methods

        public virtual Task<TransactionSearchModel> PrepareTransactionSearchModelAsync(TransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            return Task.FromResult(searchModel);
        }
        public virtual async Task<TransactionListModel> PrepareTransactionListModelAsync(TransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get transactions
            var transactions = await _transactionService.GetAllTransactionsAsync(startOnUtc: searchModel.CreatedOnFrom,
                endOnUtc: searchModel.CreatedOnTo,
                statusId: searchModel.StatusId,
                transactionTypeId: searchModel.TrancationTypeId,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new TransactionListModel().PrepareToGridAsync(searchModel, transactions, () =>
            {
                //fill in model values from the entity
                return transactions.SelectAwait(async vendor =>
                {
                    var vendorModel = vendor.ToModel<TransactionModel>();

                    return vendorModel;
                });
            });

            return model;
        }
        public virtual async Task<TransactionModel> PrepareTransactionModelAsync(TransactionModel model, Transaction transaction)
        {
            if (transaction != null)
            {
                //fill in model values from the entity
                if (model == null)
                {
                    model = transaction.ToModel<TransactionModel>();
                }
            }

            model.AvaialableStatus = new List<SelectListItem>()
            {
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(Status.Completed),Value=$"{(int)Status.Completed}" },
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(Status.Pending),Value=$"{(int)Status.Pending}"},
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(Status.Removed),Value=$"{(int)Status.Removed}"},
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(Status.Declined),Value=$"{(int)Status.Declined}"},
            };

            model.AvaialableTransactionType = new List<SelectListItem>()
            {
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(TransactionType.Debit),Value=$"{(int)TransactionType.Debit}" },
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(TransactionType.Credit),Value=$"{(int)TransactionType.Credit}"},
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(TransactionType.Voided),Value=$"{(int)TransactionType.Voided}"},
            };
            return model;
        }

        #endregion
    }
}
