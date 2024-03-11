using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Transaction;
using Nop.Core.Http.Extensions;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Transactions;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Transactions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class TransactionController : BaseAdminController
    {
        #region Fields

        private readonly IPermissionService _permissionService;
        private readonly ITransactionModelFactory _transactionModelFactory;
        private readonly ITransactionService _transactionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly ICustomerService _customerService;
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly CustomerSettings _customerSettings;
        private readonly IStoreContext _storeContext;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IPaymentService _paymentService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWithdrawService _withdrawService;

        #endregion

        #region Ctor

        public TransactionController(IPermissionService permissionService,
            ITransactionModelFactory transactionModelFactory,
            ITransactionService transactionService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            ICustomerService customerService,
            IProductService productService,
            IShoppingCartService shoppingCartService,
            CustomerSettings customerSettings,
            IStoreContext storeContext,
            IOrderProcessingService orderProcessingService,
            IPaymentService paymentService,
            IGenericAttributeService genericAttributeService,
            IWithdrawService withdrawService)
        {
            _permissionService = permissionService;
            _transactionModelFactory = transactionModelFactory;
            _transactionService = transactionService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _customerService = customerService;
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _customerSettings = customerSettings;
            _storeContext = storeContext;
            _orderProcessingService = orderProcessingService;
            _paymentService = paymentService;
            _genericAttributeService = genericAttributeService;
            _withdrawService = withdrawService;
        }

        #endregion

        #region Methods

        public virtual IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //prepare model
            var model = await _transactionModelFactory.PrepareTransactionSearchModelAsync(new TransactionSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(TransactionSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _transactionModelFactory.PrepareTransactionListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create(int customerId)
        {
            var customer = await _customerService.GetCustomerByIdAsync(customerId);
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();
            else if (customer is null ||
                (customer?.Deleted ?? true) ||
                !await _customerService.IsRegisteredAsync(customer))
                return RedirectToAction("List");
            else if (!customer.Active ||
                !customer.Verified)
                return RedirectToAction("Edit", "Customer", new { id = customerId });

            //prepare model
            var model = await _transactionModelFactory.PrepareTransactionModelAsync(new TransactionModel(customerId), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(TransactionModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            if (model.TransactionAmount.Equals(default))
            {
                ModelState.AddModelError("TransactionAmount", await _localizationService.GetResourceAsync("Admin.Transaction.ZeroAmountCannotBeTransact"));
            }

            if (ModelState.IsValid)
            {
                var customer = await _customerService.GetCustomerByIdAsync(model.CustomerId);
                switch (model.TransactionType)
                {
                    case TransactionType.Credit:
                        {
                            var product = await _productService.GetProductByIdAsync(_customerSettings.TransactionProductId);
                            if (product is null)
                            {
                                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Customer.Invest.TransactionFailed"));
                            }
                            else
                            {
                                var store = await _storeContext.GetCurrentStoreAsync();

                                //clear shopping cart
                                var cart = await _shoppingCartService.GetShoppingCartAsync(customer: customer,
                                    storeId: store.Id);
                                await Task.WhenAll(cart.Select(sci => _shoppingCartService.DeleteShoppingCartItemAsync(sci, false)));

                                var warnings = await _shoppingCartService.AddToCartAsync(customer: customer,
                                    product: product,
                                    shoppingCartType: ShoppingCartType.ShoppingCart,
                                    storeId: store.Id,
                                    customerEnteredPrice: model.TransactionAmount,
                                    addRequiredProducts: false
                                    );
                                foreach (var warning in warnings)
                                {
                                    ModelState.AddModelError("", warning);
                                }

                                if (ModelState.IsValid)
                                {
                                    cart = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, store.Id);
                                    //place order
                                    var processPaymentRequest = new ProcessPaymentRequest();
                                    _paymentService.GenerateOrderGuid(processPaymentRequest);
                                    processPaymentRequest.StoreId = store.Id;
                                    processPaymentRequest.CustomerId = customer.Id;
                                    processPaymentRequest.PaymentMethodSystemName = await _genericAttributeService.GetAttributeAsync<string>(customer,
                                        NopCustomerDefaults.SelectedPaymentMethodAttribute, store.Id);
                                    var placeOrderResult = await _orderProcessingService.PlaceOrderAsync(processPaymentRequest, true);
                                    if (placeOrderResult.Success)
                                    {
                                        var creditTransaction = await _transactionService.MakeTransactionAfterOrderCreation(placeOrderResult.PlacedOrder, customer);
                                        if (creditTransaction is not null)
                                        {
                                            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Customer.Invest.Transaction.Successfull"));
                                            await _customerActivityService.InsertActivityAsync("TransactionLog",
                                                string.Format(await _localizationService.GetResourceAsync("ActivityLog.Customer.Invest.Transaction.Pending"), creditTransaction.TransactionAmount), creditTransaction);

                                            model.Id = creditTransaction.Id;
                                        }
                                        else
                                        {
                                            _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Customer.Invest.Transaction.Unsuccessfull"));
                                        }
                                    }

                                    //error
                                    foreach (var error in placeOrderResult.Errors)
                                        ModelState.AddModelError("", error);
                                }
                            }
                        }
                        break;
                    case TransactionType.Debit:
                        {
                            var withdrawalMethod = (await _withdrawService.GetAllWithdrawalMethodAsync(customerId: customer.Id,
                                isEnabled: true)).FirstOrDefault(x => x.Id.Equals(model.WithdrawalMethodId));
                            if (withdrawalMethod is null)
                            {
                                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Customer.Withdraw.Transaction.NoWithdrawalMethodFound"));
                            }
                            else
                            {
                                var debitTransaction = await _transactionService.MakeDebitTransaction(model.TransactionAmount, withdrawalMethod, customer);
                                if (debitTransaction is not null)
                                {
                                    await _customerActivityService.InsertActivityAsync("TransactionLog",
                                        string.Format(await _localizationService.GetResourceAsync("ActivityLog.Customer.Withdraw.Transaction.Successfull"), debitTransaction.TransactionAmount), debitTransaction);

                                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Customer.Withdraw.Transaction.Successfull"));

                                    model.Id = debitTransaction.Id;
                                }
                                else
                                {
                                    _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Customer.Withdraw.Transaction.Unsuccessfull"));
                                }
                            }
                        }
                        break;
                    case TransactionType.Voided:
                        {
                            var voidedTransaction = model.ToEntity<Transaction>();

                            voidedTransaction.CreatedOnUtc = DateTime.UtcNow;
                            voidedTransaction.UpdatedOnUtc = voidedTransaction.CreatedOnUtc;
                            await _transactionService.InsertTransactionAsync(voidedTransaction);

                            //activity log
                            await _customerActivityService.InsertActivityAsync("addnewtransaction",
                                string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTransaction"), voidedTransaction.Id), voidedTransaction);

                            model.Id = voidedTransaction.Id;
                        }
                        break;
                }


                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Transactions.Added"));

                if (!continueEditing)
                    return RedirectToAction("Edit", "Customer", new { id = model.CustomerId });

                return RedirectToAction("Edit", new { id = model.Id });
            }

            //prepare model
            model = await _transactionModelFactory.PrepareTransactionModelAsync(model, null);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //try to get a transaction with the specified id
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _transactionModelFactory.PrepareTransactionModelAsync(null, transaction);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(TransactionModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //try to get a transaction with the specified id
            var transaction = await _transactionService.GetTransactionByIdAsync(model.Id);
            if (transaction == null)
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    var previousTransactionStatus = transaction.Status;
                    //var transactionUpdated = model.ToEntity<Transaction>();
                    transaction.TransactionAmount = model.TransactionAmount;
                    transaction.TransactionNote = model.TransactionNote;
                    transaction.TransactionTypeId = model.TransactionTypeId;
                    if (model.TransactionTypeId.Equals((int)TransactionType.Debit))
                    {
                        transaction.StatusId = model.StatusId;
                    }
                    if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRollBackOrder))
                    {
                        transaction.CreatedOnUtc = model.CreateOnUtc;
                    }
                    transaction.UpdatedOnUtc = DateTime.UtcNow;

                    //transactionUpdated.CustomerId = transaction.CustomerId;
                    //transactionUpdated.WithdrawalMethodId = transaction.WithdrawalMethodId;
                    //transactionUpdated.CreatedOnUtc = await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageRollBackOrder)
                    //    ? model.CreateOnUtc
                    //    : transaction.CreatedOnUtc;

                    await _transactionService.UpdateTransactionAsync(transaction);

                    //only if any debit request is approved by the admin (change status to complete)
                    if (transaction.Status.Equals(Status.Completed) &&
                        previousTransactionStatus.Equals(Status.Pending))
                    {
                        ////return error if debit request amount is not enough in the balance
                        //if (transaction.TransactionAmount > (customer.CurrentBalance + customer.InvestedAmount))
                        //{
                        //    throw new Exception(await _localizationService.GetResourceAsync("Customer.Withdraw.Transaction.NotEnoughBalance"));
                        //}

                        ////first deduct debit amount from the current balance
                        //customer.CurrentBalance -= Math.Abs(model.TransactionAmount);

                        ////if current balance is not enough for the debit amount
                        //if (customer.CurrentBalance < 0)
                        //{
                        //    //then deduct it from invested amount
                        //    customer.InvestedAmount += customer.CurrentBalance;
                        //    customer.CurrentBalance = Math.Max(customer.CurrentBalance, 0);
                        //}

                        //await _customerService.UpdateCustomerAsync(customer);

                        if (transaction.TransactionType.Equals(TransactionType.Credit))
                        {
                            await _transactionService.MarkCreditTransactionAsCompletedAsync(transaction: transaction, sendEmail: false);
                        }
                        else if (transaction.TransactionType.Equals(TransactionType.Debit))
                        {
                            var customer = await _customerService.GetCustomerByIdAsync(transaction.CustomerId);
                            await _customerActivityService.InsertActivityAsync(customer, "TransactionLog",
                                string.Format(await _localizationService.GetResourceAsync("ActivityLog.Customer.Withdraw.Transaction.Successfull"), transaction.TransactionAmount), transaction);
                        }
                    }

                    //send mail for the new updated status
                    if (!previousTransactionStatus.Equals(transaction.Status))
                    {
                        await _transactionService.SendEmailOnTransactionAsync(transaction);
                    }

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Transactions.Updated"));

                    if (!continueEditing)
                        return RedirectToAction("List");

                    return RedirectToAction("Edit", new { id = transaction.Id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
            //prepare model
            model = await _transactionModelFactory.PrepareTransactionModelAsync(model, transaction);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction) ||
                !await _permissionService.AuthorizeAsync(StandardPermissionProvider.DeleteTransaction))
                return AccessDeniedView();

            //try to get a transaction with the specified id
            var transaction = await _transactionService.GetTransactionByIdAsync(id);
            if (transaction == null)
                return RedirectToAction("List");
            try
            {
                //delete a transaction
                await _transactionService.DeleteTransactionAsync(transaction);
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification(ex.Message);
                return RedirectToAction("Edit", new { id = id });
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteTransaction",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTransaction"), transaction.Id), transaction);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Transactions.Deleted"));

            return RedirectToAction("List");
        }

        #region Return Transaction

        public virtual async Task<IActionResult> ReturnList()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //prepare model
            var model = await _transactionModelFactory.PrepareReturnTransactionSearchModelAsync(new ReturnTransactionSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> ReturnList(ReturnTransactionSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //prepare model
            var model = await _transactionModelFactory.PrepareReturnTransactionListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

        #endregion
    }
}
