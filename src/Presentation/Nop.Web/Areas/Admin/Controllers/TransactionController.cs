using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Transaction;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
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

        #endregion

        #region Ctor

        public TransactionController(IPermissionService permissionService, 
            ITransactionModelFactory transactionModelFactory,
            ITransactionService transactionService,
            ICustomerActivityService customerActivityService, 
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _permissionService = permissionService;
            _transactionModelFactory = transactionModelFactory;
            _transactionService = transactionService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }

        #endregion

        #region Transactions

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

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //prepare model
            var model = await _transactionModelFactory.PrepareTransactionModelAsync(new TransactionModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual async Task<IActionResult> Create(TransactionModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var transaction = model.ToEntity<Transaction>();
                transaction.CreatedOnUtc = DateTime.UtcNow;
                transaction.UpdatedOnUtc = transaction.CreatedOnUtc;
                await _transactionService.InsertTransactionAsync(transaction);

                //activity log
                await _customerActivityService.InsertActivityAsync("AddNewTransaction",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTransaction"), transaction.Id), transaction);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Transactions.Added"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = transaction.Id });
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

            if (ModelState.IsValid)
            {
                transaction = model.ToEntity(transaction);
                await _transactionService.UpdateTransactionAsync(transaction);

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Transactions.Updated"));

                if (!continueEditing)
                    return RedirectToAction("List");

                return RedirectToAction("Edit", new { id = transaction.Id });
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

            //delete a transaction
            await _transactionService.DeleteTransactionAsync(transaction);

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteTransaction",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteTransaction"), transaction.Id), transaction);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Transactions.Deleted"));

            return RedirectToAction("List");
        }

        #endregion
    }
}
