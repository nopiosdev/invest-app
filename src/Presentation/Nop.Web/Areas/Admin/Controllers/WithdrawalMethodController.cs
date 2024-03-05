using System.Transactions;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using Nop.Core.Domain.Transaction;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Transactions;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.WithdrawMethods;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Areas.Admin.Controllers
{
    public class WithdrawalMethodController : BaseAdminController
    {
        #region Fields

        private readonly IWithdrawalMethodModelFactory _withdrawalMethodModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly IWithdrawService _withdrawService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly INotificationService _notificationService;
        private readonly Services.Logging.ILogger _logger;

        #endregion

        #region Ctor

        public WithdrawalMethodController(IWithdrawalMethodModelFactory withdrawalMethodModelFactory,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            IWithdrawService withdrawService,
            ICustomerActivityService customerActivityService,
            INotificationService notificationService,
            Services.Logging.ILogger logger)
        {
            _withdrawalMethodModelFactory = withdrawalMethodModelFactory;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _withdrawService = withdrawService;
            _customerActivityService = customerActivityService;
            _notificationService = notificationService;
            _logger = logger;
        }

        #endregion

        #region Methods

        public IActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual async Task<IActionResult> List()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            var model = await _withdrawalMethodModelFactory.PrepareWithdrawMethodSearchModelAsync(new WithdrawalMethodSearchModel());

            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> List(WithdrawalMethodSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return await AccessDeniedDataTablesJson();

            var model = await _withdrawalMethodModelFactory.PrepareWithdrawMethodListModelAsync(searchModel);

            return Json(model);
        }

        public virtual async Task<IActionResult> Create()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            var model = await _withdrawalMethodModelFactory.PrepareWithdrawalMethodModelAsync(new WithdrawalMethodModel(), null);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [HttpPost]
        public virtual async Task<IActionResult> Create(WithdrawalMethodModel model, bool continueEditing)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            if (string.IsNullOrEmpty(model.Name))
            {
                ModelState.AddModelError("Name", await _localizationService.GetResourceAsync("Admin.WithdrawalMethod.Name.Required"));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var entity = model.ToEntity<WithdrawalMethod>();
                    await _withdrawService.InsertWithdrawalMethodAsync(entity);

                    //activity log
                    await _customerActivityService.InsertActivityAsync("AddNewTransaction",
                    string.Format(await _localizationService.GetResourceAsync("ActivityLog.AddNewTransaction"), entity.Id), entity);
                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Transactions.Added"));

                    if (!continueEditing)
                        return RedirectToAction("List");

                    return RedirectToAction("Edit", new { id = entity.Id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            model = await _withdrawalMethodModelFactory.PrepareWithdrawalMethodModelAsync(model, null);

            return View(model);
        }

        public virtual async Task<IActionResult> Edit(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //try to get a WithdrawalMethod with the specified id
            var entity = await _withdrawService.GetWithdrawalMethodByIdAsync(id);
            if (entity == null)
                return RedirectToAction("List");

            //prepare model
            var model = await _withdrawalMethodModelFactory.PrepareWithdrawalMethodModelAsync(null, entity);

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual async Task<IActionResult> Edit(WithdrawalMethodModel model, bool continueEditing, IFormCollection form)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //try to get a WithdrawalMethod with the specified id
            var entity = await _withdrawService.GetWithdrawalMethodByIdAsync(model.Id);
            if (entity == null)
                return RedirectToAction("List");

            try
            {
                if (ModelState.IsValid)
                {
                    entity = model.ToEntity<WithdrawalMethod>();
                    await _withdrawService.UpdateWithdrawalMethodAsync(entity);

                    _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.WithdrawalMethod.Updated"));

                    if (!continueEditing)
                        return RedirectToAction("List");

                    return RedirectToAction("Edit", new { id = entity.Id });
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }

            //prepare model
            model = await _withdrawalMethodModelFactory.PrepareWithdrawalMethodModelAsync(model, entity);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageTransaction))
                return AccessDeniedView();

            //try to get a WithdrawalMethod with the specified id
            var entity = await _withdrawService.GetWithdrawalMethodByIdAsync(id);
            if (entity == null)
                return RedirectToAction("List");
            try
            {
                //delete a transaction
                await _withdrawService.DeleteWithdrawalMethodAsync(entity);
            }
            catch (Exception ex)
            {
                _notificationService.ErrorNotification(ex.Message);
                return RedirectToAction("Edit", new { id = id });
            }

            //activity log
            await _customerActivityService.InsertActivityAsync("DeleteWithdrawalMethod",
                string.Format(await _localizationService.GetResourceAsync("ActivityLog.DeleteWithdrawalMethod"), entity.Id), entity);

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.WithdrawalMethods.Deleted"));

            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual async Task<IActionResult> WithdrawalMethodFieldList(WithdrawalMethodFieldSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _withdrawalMethodModelFactory.PrepareWithdrawalMethodFieldListModelAsync(searchModel);

            return Json(model);
        }

        [HttpPost]
        public virtual async Task<IActionResult> WithdrawalMethodFieldAdd(WithdrawalMethodFieldModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            bool success = default;

            if (!string.IsNullOrEmpty(model.FieldName))
            {
                try
                {
                    var entity = model.ToEntity<WithdrawalMethodField>();
                    await _withdrawService.InsertWithdrawalMethodFieldAsync(entity);

                    success = true;
                }
                catch (Exception ex)
                {
                    await _logger.ErrorAsync(ex.Message, ex, null);
                }
            }

            return Json(new { Result = success });
        }

        [HttpPost]
        public virtual async Task<IActionResult> WithdrawalMethodFieldUpdate(WithdrawalMethodFieldModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a WithdrawalMethodField picture with the specified id
            var entity = await _withdrawService.GetWithdrawalMethodFieldByIdAsync(model.Id)
                ?? throw new ArgumentException("No WithdrawalMethodField found with the specified id");

            var withdrawalMethodId = entity.WithdrawalMethodId;
            entity = model.ToEntity(entity);
            entity.WithdrawalMethodId = withdrawalMethodId;

            await _withdrawService.UpdateWithdrawalMethodFieldAsync(entity);

            return new NullJsonResult();
        }

        [HttpPost]
        public virtual async Task<IActionResult> WithdrawalMethodFieldDelete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //try to get a WithdrawalMethodField picture with the specified id
            var entity = await _withdrawService.GetWithdrawalMethodFieldByIdAsync(id)
                ?? throw new ArgumentException("No WithdrawalMethodField found with the specified id");

            await _withdrawService.DeleteWithdrawalMethodFieldAsync(entity);
            return new NullJsonResult();
        }

        #endregion
    }
}
