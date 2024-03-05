using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Nop.Core;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;

namespace Nop.Web.Framework.Mvc.Filters
{
    public sealed class AuthorizeCustomerAttribute : TypeFilterAttribute
    {
        #region Ctor

        public AuthorizeCustomerAttribute(bool ignore = false) : base(typeof(AuthorizeCustomerFilter))
        {
            IgnoreFilter = ignore;
            Arguments = new object[] { ignore };
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether to ignore the execution of filter actions
        /// </summary>
        public bool IgnoreFilter { get; }

        #endregion

        #region Nested filter

        /// <summary>
        /// Represents a filter that confirms access to the admin panel
        /// </summary>
        private class AuthorizeCustomerFilter : IAsyncAuthorizationFilter
        {
            #region Fields

            protected readonly bool _ignoreFilter;
            protected readonly IWorkContext _workContext;
            protected readonly ICustomerService _customerService;
            protected readonly IWebHelper _webHelper;
            protected readonly INotificationService _notificationService;
            protected readonly ILocalizationService _localizationService;

            #endregion

            #region Ctor

            public AuthorizeCustomerFilter(bool ignoreFilter,
                IWorkContext workContext,
                ICustomerService customerService,
                IWebHelper webHelper,
                INotificationService notificationService,
                ILocalizationService localizationService)
            {
                _ignoreFilter = ignoreFilter;
                _workContext = workContext;
                _customerService = customerService;
                _webHelper = webHelper;
                _notificationService = notificationService;
                _localizationService = localizationService;
            }

            #endregion

            #region Utilities

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>
            private async Task AuthorizeCustomerAsync(AuthorizationFilterContext context)
            {
                if (context == null)
                    throw new ArgumentNullException(nameof(context));

                if (!DataSettingsManager.IsDatabaseInstalled())
                    return;

                //check whether this filter has been overridden for the action
                var actionFilter = context.ActionDescriptor.FilterDescriptors
                    .Where(filterDescriptor => filterDescriptor.Scope == FilterScope.Action)
                    .Select(filterDescriptor => filterDescriptor.Filter)
                    .OfType<AuthorizeCustomerAttribute>()
                    .FirstOrDefault();

                //ignore filter (the action is available even if a customer hasn't access to the method)
                if (actionFilter?.IgnoreFilter ?? _ignoreFilter)
                    return;

                //there is AdminAuthorizeFilter, so check access
                if (context.Filters.Any(filter => filter is AuthorizeCustomerFilter))
                {
                    var customer = await _workContext.GetCurrentCustomerAsync();
                    if (!await _customerService.IsRegisteredAsync(customer))
                    {
                        context.Result = new ChallengeResult();
                    }
                    else if (!customer.Verified)
                    {
                        _notificationService.ErrorNotification(await _localizationService.GetResourceAsync("Customer.NotVerified"));
                        context.Result = new RedirectToRouteResult("Homepage", null);
                    }
                }
            }

            #endregion

            #region Methods

            /// <summary>
            /// Called early in the filter pipeline to confirm request is authorized
            /// </summary>
            /// <param name="context">Authorization filter context</param>
            /// <returns>A task that represents the asynchronous operation</returns>

            public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
            {
                await AuthorizeCustomerAsync(context);
            }

            #endregion
        }

        #endregion

    }
}
