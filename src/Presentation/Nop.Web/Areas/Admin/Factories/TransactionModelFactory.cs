using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Transaction;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Transactions;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Transactions;
using Nop.Web.Areas.Admin.Models.Vendors;
using Nop.Web.Framework.Models.Extensions;
using WithdrawalMethodCustomerInfoModel = Nop.Web.Models.Customer.WithdrawalMethodCustomerInfoModel;
using ICustomerModelFactory = Nop.Web.Factories.ICustomerModelFactory;
using Nop.Services.Catalog;

namespace Nop.Web.Areas.Admin.Factories
{
    public partial class TransactionModelFactory : ITransactionModelFactory
    {
        #region Fields

        private readonly ITransactionService _transactionService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IPermissionService _permissionService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWithdrawService _withdrawService;
        private readonly Nop.Web.Factories.ICustomerModelFactory _customerModelFactory;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public TransactionModelFactory(ITransactionService transactionService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            IPermissionService permissionService,
            IDateTimeHelper dateTimeHelper,
            IWithdrawService withdrawService,
            Nop.Web.Factories.ICustomerModelFactory customerModelFactory,
            IPriceFormatter priceFormatter)
        {
            _transactionService = transactionService;
            _localizationService = localizationService;
            _customerService = customerService;
            _permissionService = permissionService;
            _dateTimeHelper = dateTimeHelper;
            _withdrawService = withdrawService;
            _customerModelFactory = customerModelFactory;
            _priceFormatter = priceFormatter;
        }


        #endregion

        #region Methods

        public virtual async Task<TransactionSearchModel> PrepareTransactionSearchModelAsync(TransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            searchModel.AvailableStatus = new List<SelectListItem>()
            {
               new SelectListItem(){Text="--Select", Value="0" },
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(Status.Completed),Value=$"{(int)Status.Completed}" },
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(Status.Pending),Value=$"{(int)Status.Pending}"},
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(Status.Removed),Value=$"{(int)Status.Removed}"},
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(Status.Declined),Value=$"{(int)Status.Declined}"},
            };

            searchModel.AvailableTransactionType = new List<SelectListItem>()
            {
               new SelectListItem(){Text="--Select", Value="0" },
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(TransactionType.Debit),Value=$"{(int)TransactionType.Debit}" },
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(TransactionType.Credit),Value=$"{(int)TransactionType.Credit}"},
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(TransactionType.Voided),Value=$"{(int)TransactionType.Voided}"},
            };

            return searchModel;
        }

        public virtual async Task<TransactionListModel> PrepareTransactionListModelAsync(TransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get transactions
            var transactions = await _transactionService.GetAllTransactionsAsync(startOnUtc: searchModel.CreatedOnFrom,
                endOnUtc: searchModel.CreatedOnTo,
                statusId: searchModel.StatusId,
                transactionTypeId: searchModel.TransactionTypeId,
                customerId: searchModel.CustomerId,
                orderBy: 2,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new TransactionListModel().PrepareToGridAsync(searchModel, transactions, () =>
            {
                //fill in model values from the entity
                return transactions
                .SelectAwait(async transaction =>
                {
                    var transactionModel = transaction.ToModel<TransactionModel>();
                    transactionModel.CreateOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(transaction.CreatedOnUtc, DateTimeKind.Utc);
                    transactionModel.TransactionTypeString = await _localizationService.GetLocalizedEnumAsync(transactionModel.TransactionType);
                    transactionModel.StatusString = await _localizationService.GetLocalizedEnumAsync(transactionModel.Status);
                    transactionModel.FormattedTransactionAmount = await _priceFormatter.FormatPriceAsync(transactionModel.TransactionAmount);

                    return transactionModel;
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
                    model.CreateOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(transaction.CreatedOnUtc, DateTimeKind.Utc);
                    model.CustomerInfo = (await _customerService.GetCustomerByIdAsync(model.CustomerId))?.Email;
                }

                if (transaction.TransactionType.Equals(TransactionType.Debit))
                {
                    var withdrawalMethod = await _withdrawService.GetWithdrawalMethodByIdAsync(transaction.WithdrawalMethodId.GetValueOrDefault());
                    //must be enabled
                    if ((withdrawalMethod?.IsEnabled ?? false) &&
                        //isRequested flag should be up to say it is a withdrawal method where amount should be sent
                        (withdrawalMethod?.IsRequested ?? false))
                    {
                        //get type of the requested withdrawal method
                        model.WithdrawalMethodModel.Id = withdrawalMethod.TypeId;
                        model.WithdrawalMethodModel.Name = await _localizationService.GetLocalizedEnumAsync(withdrawalMethod.Type);

                        var temp1 = new WithdrawalMethodCustomerInfoModel()
                        {
                            Id = withdrawalMethod.Id,
                            Name = withdrawalMethod.Name
                        };

                        var withdrawalMethodFields = await _withdrawService.GetAllWithdrawalMethodFieldAsync(withdrawalMethodId: withdrawalMethod.Id,
                            isEnabled: true);

                        foreach (var withdrawalMethodField in withdrawalMethodFields)
                        {
                            var customerWithdrawalMethod = (await _withdrawService.GetAllCustomerWithdrawalMethodAsync(customerId: transaction.CustomerId,
                                withdrawalMethodFieldId: withdrawalMethodField.Id,
                                isRequested: true))
                                .FirstOrDefault();

                            temp1.Fields.Add(new WithdrawalMethodCustomerInfoModel()
                            {
                                Id = withdrawalMethodField.Id,
                                Name = withdrawalMethodField.FieldName,
                                Value = customerWithdrawalMethod?.Value
                            });
                        }

                        model.WithdrawalMethodModel.Fields.Add(temp1);
                    }
                }
            }
            else
            {
                model.CreateOnUtc = await _dateTimeHelper.ConvertToUserTimeAsync(DateTime.UtcNow, DateTimeKind.Utc);
                var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
                if (customer is not null)
                {
                    //for the transaction create page customer ID will be sent to make reference
                    model.CustomerInfo = customer.Email;

                    await _customerModelFactory.PrepareWithdrawalMethodCustomerInfoModelAsync(model.WithdrawalMethodCustomerInfoModels, customer);
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
               new SelectListItem(){Text=await _localizationService.GetLocalizedEnumAsync(TransactionType.Debit),Value=$"{(int)TransactionType.Debit}" }
            };

            //when a new transaction is creating, only debit is allowed none other than this
            if (!model.Id.Equals(0))
            {
                model.AvaialableTransactionType.Add(new SelectListItem() { Text = await _localizationService.GetLocalizedEnumAsync(TransactionType.Credit), Value = $"{(int)TransactionType.Credit}" });
                model.AvaialableTransactionType.Add(new SelectListItem() { Text = await _localizationService.GetLocalizedEnumAsync(TransactionType.Voided), Value = $"{(int)TransactionType.Voided}" });
            }

            model.UserCanDelete = await _permissionService.AuthorizeAsync(StandardPermissionProvider.DeleteTransaction);
            model.CanChangeDate = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRollBackOrder);

            return model;
        }

        #region Return Transaction

        public virtual async Task<ReturnTransactionSearchModel> PrepareReturnTransactionSearchModelAsync(ReturnTransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //prepare page parameters
            searchModel.SetGridPageSize();

            var allReturnTransaction = await _transactionService.GetAllReturnTransactionsAsync();
            foreach (var item in allReturnTransaction)
            {
                if (!searchModel.AvailableYears.Any(x => x.Value.Equals(item.ReturnDateOnUtc.Year.ToString())))
                {
                    searchModel.AvailableYears.Add(new SelectListItem()
                    {
                        Text = item.ReturnDateOnUtc.ToString("yyyy"),
                        Value = item.ReturnDateOnUtc.Year.ToString()
                    });
                }
                if (!searchModel.AvailableMonths.Any(x => x.Value.Equals(item.ReturnDateOnUtc.Month.ToString())))
                {
                    searchModel.AvailableMonths.Add(new SelectListItem()
                    {
                        Text = item.ReturnDateOnUtc.ToString("MMMM"),
                        Value = item.ReturnDateOnUtc.Month.ToString()
                    });
                }
            }

            searchModel.AvailableMonths.Insert(0, new SelectListItem() { Text = "--Select", Value = "0" });
            searchModel.AvailableYears.Insert(0, new SelectListItem() { Text = "--Select", Value = "0" });

            return searchModel;
        }

        public virtual async Task<ReturnTransactionListModel> PrepareReturnTransactionListModelAsync(ReturnTransactionSearchModel searchModel)
        {
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get transactions
            var returnTransactions = await _transactionService.GetAllReturnTransactionsAsync(month: searchModel.Month,
                year: searchModel.Year,
                pageIndex: searchModel.Page - 1, pageSize: searchModel.PageSize);

            //prepare list model
            var model = await new ReturnTransactionListModel().PrepareToGridAsync(searchModel, returnTransactions, () =>
            {
                //fill in model values from the entity
                return returnTransactions
                .SelectAwait(async returnTransaction =>
                {
                    var transaction = await _transactionService.GetTransactionByIdAsync(returnTransaction.TransactionId);
                    var customer = await _customerService.GetCustomerByIdAsync(transaction.CustomerId);

                    var returnTransactionModel = returnTransaction.ToModel<ReturnTransactionModel>();

                    returnTransactionModel.CustomerFullName = await _customerService.GetCustomerFullNameAsync(customer);
                    returnTransactionModel.CustomerEmail = customer?.Email ?? default;
                    returnTransactionModel.CustomerId = customer?.Id ?? default;

                    var withdrawalMethod = await _withdrawService.GetWithdrawalMethodByIdAsync(transaction.WithdrawalMethodId.GetValueOrDefault());
                    if (withdrawalMethod is not null)
                    {
                        var withdrawalMethodField = (await _withdrawService.GetAllWithdrawalMethodFieldAsync(withdrawalMethodId: withdrawalMethod.Id))
                            .FirstOrDefault();
                        var customerWithdrawalMethod = (await _withdrawService.GetAllCustomerWithdrawalMethodAsync(withdrawalMethodFieldId: withdrawalMethodField.Id))
                            .FirstOrDefault();
                        returnTransactionModel.WithdrawalMethod += $"<b>{withdrawalMethod.Name}</b>" +
                            $"<br />" +
                            $"{withdrawalMethodField?.FieldName}: {customerWithdrawalMethod?.Value}";
                    }

                    //returnTransactionModel.WithdrawalMethod ??= "test";

                    return returnTransactionModel;
                });
            });

            return model;
        }


        #endregion

        #endregion
    }
}
