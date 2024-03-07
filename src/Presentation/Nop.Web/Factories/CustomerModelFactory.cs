using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Transaction;
using Nop.Core.Domain.Vendors;
using Nop.Services.Authentication.External;
using Nop.Services.Authentication.MultiFactor;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Transactions;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the customer model factory
    /// </summary>
    public partial class CustomerModelFactory : ICustomerModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly ICountryService _countryService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly IMultiFactorAuthenticationPluginManager _multiFactorAuthenticationPluginManager;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;

        //NCT Back-end dev
        private readonly ITransactionService _transactionService;
        private readonly IWithdrawService _withdrawService;
        private readonly IPriceFormatter _priceFormatter;

        #endregion

        #region Ctor

        public CustomerModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressModelFactory addressModelFactory,
            IAuthenticationPluginManager authenticationPluginManager,
            ICountryService countryService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            IMultiFactorAuthenticationPluginManager multiFactorAuthenticationPluginManager,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPermissionService permissionService,
            IPictureService pictureService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            SecuritySettings securitySettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            ITransactionService transactionService,
            IWithdrawService withdrawService,
            IPriceFormatter priceFormatter)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _externalAuthenticationService = externalAuthenticationService;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _forumSettings = forumSettings;
            _gdprSettings = gdprSettings;
            _addressModelFactory = addressModelFactory;
            _authenticationPluginManager = authenticationPluginManager;
            _countryService = countryService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _multiFactorAuthenticationPluginManager = multiFactorAuthenticationPluginManager;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _permissionService = permissionService;
            _pictureService = pictureService;
            _productService = productService;
            _returnRequestService = returnRequestService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _securitySettings = securitySettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
            _transactionService = transactionService;
            _withdrawService = withdrawService;
            _priceFormatter = priceFormatter;
        }

        #endregion

        #region Utilities

        /// <returns>A task that represents the asynchronous operation</returns>
        protected virtual async Task<GdprConsentModel> PrepareGdprConsentModelAsync(GdprConsent consent, bool accepted)
        {
            if (consent == null)
                throw new ArgumentNullException(nameof(consent));

            var requiredMessage = await _localizationService.GetLocalizedAsync(consent, x => x.RequiredMessage);
            return new GdprConsentModel
            {
                Id = consent.Id,
                Message = await _localizationService.GetLocalizedAsync(consent, x => x.Message),
                IsRequired = consent.IsRequired,
                RequiredMessage = !string.IsNullOrEmpty(requiredMessage) ? requiredMessage : $"'{consent.Message}' is required",
                Accepted = accepted
            };
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the customer info model
        /// </summary>
        /// <param name="model">Customer info model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer info model
        /// </returns>
        public virtual async Task<CustomerInfoModel> PrepareCustomerInfoModelAsync(CustomerInfoModel model, Customer customer,
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

            var store = await _storeContext.GetCurrentStoreAsync();
            if (!excludeProperties)
            {
                model.VatNumber = customer.VatNumber;
                model.FirstName = customer.FirstName;
                model.LastName = customer.LastName;
                model.Gender = customer.Gender;
                var dateOfBirth = customer.DateOfBirth;
                if (dateOfBirth.HasValue)
                {
                    var currentCalendar = CultureInfo.CurrentCulture.Calendar;

                    model.DateOfBirthDay = currentCalendar.GetDayOfMonth(dateOfBirth.Value);
                    model.DateOfBirthMonth = currentCalendar.GetMonth(dateOfBirth.Value);
                    model.DateOfBirthYear = currentCalendar.GetYear(dateOfBirth.Value);
                }
                model.Company = customer.Company;
                model.StreetAddress = customer.StreetAddress;
                model.StreetAddress2 = customer.StreetAddress2;
                model.ZipPostalCode = customer.ZipPostalCode;
                model.City = customer.City;
                model.County = customer.County;
                model.CountryId = customer.CountryId;
                model.StateProvinceId = customer.StateProvinceId;
                model.Phone = customer.Phone;
                model.Fax = customer.Fax;

                //newsletter
                var newsletter = await _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreIdAsync(customer.Email, store.Id);
                model.Newsletter = newsletter != null && newsletter.Active;

                model.Signature = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SignatureAttribute);

                model.Email = customer.Email;
                model.Username = customer.Username;
            }
            else
            {
                if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                    model.Username = customer.Username;
            }

            if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                model.EmailToRevalidate = customer.EmailToRevalidate;

            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = await _localizationService.GetLocalizedEnumAsync(customer.VatNumberStatus);
            model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
            model.LastNameEnabled = _customerSettings.LastNameEnabled;
            model.FirstNameRequired = _customerSettings.FirstNameRequired;
            model.LastNameRequired = _customerSettings.LastNameRequired;
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.CompanyRequired = _customerSettings.CompanyRequired;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CityRequired = _customerSettings.CityRequired;
            model.CountyEnabled = _customerSettings.CountyEnabled;
            model.CountyRequired = _customerSettings.CountyRequired;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.CountryRequired = _customerSettings.CountryRequired;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.PhoneRequired = _customerSettings.PhoneRequired;
            model.FaxEnabled = _customerSettings.FaxEnabled;
            model.FaxRequired = _customerSettings.FaxRequired;
            model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
            model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

            //external authentication
            var currentCustomer = await _workContext.GetCurrentCustomerAsync();
            model.AllowCustomersToRemoveAssociations = _externalAuthenticationSettings.AllowCustomersToRemoveAssociations;
            model.NumberOfExternalAuthenticationProviders = (await _authenticationPluginManager
                .LoadActivePluginsAsync(currentCustomer, store.Id))
                .Count;
            foreach (var record in await _externalAuthenticationService.GetCustomerExternalAuthenticationRecordsAsync(customer))
            {
                var authMethod = await _authenticationPluginManager
                    .LoadPluginBySystemNameAsync(record.ProviderSystemName, currentCustomer, store.Id);
                if (!_authenticationPluginManager.IsPluginActive(authMethod))
                    continue;

                model.AssociatedExternalAuthRecords.Add(new CustomerInfoModel.AssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                        ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                    AuthMethodName = await _localizationService.GetLocalizedFriendlyNameAsync(authMethod, currentLanguage.Id)
                });
            }

            //custom customer attributes
            var customAttributes = await PrepareCustomCustomerAttributesAsync(customer, overrideCustomCustomerAttributesXml);
            foreach (var attribute in customAttributes)
                model.CustomerAttributes.Add(attribute);

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var accepted = await _gdprService.IsConsentAcceptedAsync(consent.Id, currentCustomer.Id);
                    model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, accepted.HasValue && accepted.Value));
                }
            }

            //NCT Back-end dev
            model.AvailableGoal = new List<SelectListItem>()
            {
                new SelectListItem(){Text="Growth",Value="1" },
                new SelectListItem(){Text="Investment",Value="2"},
                new SelectListItem(){Text="Retirement",Value="3"},
                new SelectListItem(){Text="Savings",Value="4"},
                new SelectListItem(){Text="Income",Value="5"},
                new SelectListItem(){Text="Something else",Value="6"},
            };
            model.AvailableTimeline = new List<SelectListItem>()
            {
                new SelectListItem(){Text="1-5 years",Value="1" },
                new SelectListItem(){Text="5-10 years",Value="2"},
                new SelectListItem(){Text="More than 10 years",Value="3"},
            };
            model.AvailableExperience = new List<SelectListItem>()
            {
                new SelectListItem(){Text="Not Much",Value="1"},
                new SelectListItem(){Text="I'm okay",Value="2"},
                new SelectListItem(){Text="I'm an expert",Value="3"},
            };
            model.AvailableRiskTolerance = new List<SelectListItem>()
            {
                new SelectListItem(){Text="High",Value="1"},
                new SelectListItem(){Text="Medium",Value="2"},
                new SelectListItem(){Text="Low",Value="3"},
            };
            model.AvailableInvestmentApproach = new List<SelectListItem>()
            {
                new SelectListItem(){Text="I do not like to lose money",Value="1"},
                new SelectListItem(){Text="Willing to risk losing money in order to make more",Value="2"},
                new SelectListItem(){Text="Not important to me",Value="3"},
            };
            foreach (PaymentTypeEnum enumValues in Enum.GetValues(typeof(PaymentTypeEnum)))
            {
                model.AvailablePaymentType.Add(new SelectListItem()
                {
                    Text = await _localizationService.GetLocalizedEnumAsync(enumValues),
                    Value = $"{(int)enumValues}",
                });
            }
            model.GoalId = customer.GoalId;
            model.TimelineId = customer.TimelineId;
            model.ExperienceId = customer.ExperienceId;
            model.RiskToleranceId = customer.RiskToleranceId;
            model.InvestmentApproachId = customer.InvestmentApproachId;
            model.EmailAlert = customer.EmailAlert;
            model.TextAlert = customer.TextAlert;
            model.DefaultWithdrawalMethodId = customer.DefaultWithdrawalMethodId;
            model.PaymentType = customer.PaymentTypeId;

            await this.PrepareWithdrawalMethodCustomerInfoModelAsync(model.WithdrawalMethodModel, customer);

            //find address (ensure that it belongs to the current customer)
            var address = await _customerService.GetAddressesByCustomerIdAsync(customer.Id);
            //prepare address model
            await _addressModelFactory.PrepareAddressModelAsync(model.Address,
                address: address.FirstOrDefault(),
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));

            return model;
        }

        /// <summary>
        /// Prepare the customer register model
        /// </summary>
        /// <param name="model">Customer register model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer register model
        /// </returns>
        public virtual async Task<RegisterModel> PrepareRegisterModelAsync(RegisterModel model, bool excludeProperties,
            string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var customer = await _workContext.GetCurrentCustomerAsync();

            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == (await _dateTimeHelper.GetCurrentTimeZoneAsync()).Id) });

            //VAT
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            if (_taxSettings.EuVatEnabled && _taxSettings.EuVatEnabledForGuests)
                model.VatNumber = customer.VatNumber;

            //form fields
            model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
            model.LastNameEnabled = _customerSettings.LastNameEnabled;
            model.FirstNameRequired = _customerSettings.FirstNameRequired;
            model.LastNameRequired = _customerSettings.LastNameRequired;
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.CompanyRequired = _customerSettings.CompanyRequired;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CityRequired = _customerSettings.CityRequired;
            model.CountyEnabled = _customerSettings.CountyEnabled;
            model.CountyRequired = _customerSettings.CountyRequired;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.CountryRequired = _customerSettings.CountryRequired;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.PhoneRequired = _customerSettings.PhoneRequired;
            model.FaxEnabled = _customerSettings.FaxEnabled;
            model.FaxRequired = _customerSettings.FaxRequired;
            model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
            model.AcceptPrivacyPolicyEnabled = _customerSettings.AcceptPrivacyPolicyEnabled;
            model.AcceptPrivacyPolicyPopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
            model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            model.EnteringEmailTwice = _customerSettings.EnteringEmailTwice;
            if (setDefaultValues)
            {
                //enable newsletter by default
                model.Newsletter = _customerSettings.NewsletterTickedByDefault;
            }

            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectCountry"), Value = "0" });
                var currentLanguage = await _workContext.GetWorkingLanguageAsync();
                foreach (var c in await _countryService.GetAllCountriesAsync(currentLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = await _localizationService.GetLocalizedAsync(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = (await _stateProvinceService.GetStateProvincesByCountryIdAsync(model.CountryId, currentLanguage.Id)).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetResourceAsync("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = await _localizationService.GetLocalizedAsync(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = await _localizationService.GetResourceAsync(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            //custom customer attributes
            var customAttributes = await PrepareCustomCustomerAttributesAsync(customer, overrideCustomCustomerAttributesXml);
            foreach (var attribute in customAttributes)
                model.CustomerAttributes.Add(attribute);

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = (await _gdprService.GetAllConsentsAsync()).Where(consent => consent.DisplayDuringRegistration).ToList();
                foreach (var consent in consents)
                {
                    model.GdprConsents.Add(await PrepareGdprConsentModelAsync(consent, false));
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the login model
        /// </returns>
        public virtual Task<LoginModel> PrepareLoginModelAsync(bool? checkoutAsGuest)
        {
            var model = new LoginModel
            {
                UsernamesEnabled = _customerSettings.UsernamesEnabled,
                RegistrationType = _customerSettings.UserRegistrationType,
                CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <param name="model">Password recovery model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the password recovery model
        /// </returns>
        public virtual Task<PasswordRecoveryModel> PreparePasswordRecoveryModelAsync(PasswordRecoveryModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage;

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <param name="returnUrl">URL to redirect</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the register result model
        /// </returns>
        public virtual async Task<RegisterResultModel> PrepareRegisterResultModelAsync(int resultId, string returnUrl)
        {
            var resultText = (UserRegistrationType)resultId switch
            {
                UserRegistrationType.Disabled => await _localizationService.GetResourceAsync("Account.Register.Result.Disabled"),
                UserRegistrationType.Standard => await _localizationService.GetResourceAsync("Account.Register.Result.Standard"),
                UserRegistrationType.AdminApproval => await _localizationService.GetResourceAsync("Account.Register.Result.AdminApproval"),
                UserRegistrationType.EmailValidation => await _localizationService.GetResourceAsync("Account.Register.Result.EmailValidation"),
                _ => null
            };

            var model = new RegisterResultModel
            {
                Result = resultText,
                ReturnUrl = returnUrl
            };

            return model;
        }

        /// <summary>
        /// Prepare the customer navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer navigation model
        /// </returns>
        public virtual async Task<CustomerNavigationModel> PrepareCustomerNavigationModelAsync(int selectedTabId = 0)
        {
            var model = new CustomerNavigationModel();

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerInfo",
                Title = await _localizationService.GetResourceAsync("Account.CustomerInfo"),
                Tab = (int)CustomerNavigationEnum.Info,
                ItemClass = "customer-info"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerAddresses",
                Title = await _localizationService.GetResourceAsync("Account.CustomerAddresses"),
                Tab = (int)CustomerNavigationEnum.Addresses,
                ItemClass = "customer-addresses"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerOrders",
                Title = await _localizationService.GetResourceAsync("Account.CustomerOrders"),
                Tab = (int)CustomerNavigationEnum.Orders,
                ItemClass = "customer-orders"
            });

            var store = await _storeContext.GetCurrentStoreAsync();
            var customer = await _workContext.GetCurrentCustomerAsync();

            if (_orderSettings.ReturnRequestsEnabled &&
                (await _returnRequestService.SearchReturnRequestsAsync(store.Id,
                    customer.Id, pageIndex: 0, pageSize: 1)).Any())
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerReturnRequests",
                    Title = await _localizationService.GetResourceAsync("Account.CustomerReturnRequests"),
                    Tab = (int)CustomerNavigationEnum.ReturnRequests,
                    ItemClass = "return-requests"
                });
            }

            if (!_customerSettings.HideDownloadableProductsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerDownloadableProducts",
                    Title = await _localizationService.GetResourceAsync("Account.DownloadableProducts"),
                    Tab = (int)CustomerNavigationEnum.DownloadableProducts,
                    ItemClass = "downloadable-products"
                });
            }

            if (!_customerSettings.HideBackInStockSubscriptionsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerBackInStockSubscriptions",
                    Title = await _localizationService.GetResourceAsync("Account.BackInStockSubscriptions"),
                    Tab = (int)CustomerNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

            if (_rewardPointsSettings.Enabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerRewardPoints",
                    Title = await _localizationService.GetResourceAsync("Account.RewardPoints"),
                    Tab = (int)CustomerNavigationEnum.RewardPoints,
                    ItemClass = "reward-points"
                });
            }

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerChangePassword",
                Title = await _localizationService.GetResourceAsync("Account.ChangePassword"),
                Tab = (int)CustomerNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAvatar",
                    Title = await _localizationService.GetResourceAsync("Account.Avatar"),
                    Tab = (int)CustomerNavigationEnum.Avatar,
                    ItemClass = "customer-avatar"
                });
            }

            if (_forumSettings.ForumsEnabled && _forumSettings.AllowCustomersToManageSubscriptions)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerForumSubscriptions",
                    Title = await _localizationService.GetResourceAsync("Account.ForumSubscriptions"),
                    Tab = (int)CustomerNavigationEnum.ForumSubscriptions,
                    ItemClass = "forum-subscriptions"
                });
            }
            if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerProductReviews",
                    Title = await _localizationService.GetResourceAsync("Account.CustomerProductReviews"),
                    Tab = (int)CustomerNavigationEnum.ProductReviews,
                    ItemClass = "customer-reviews"
                });
            }
            if (_vendorSettings.AllowVendorsToEditInfo && await _workContext.GetCurrentVendorAsync() != null)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerVendorInfo",
                    Title = await _localizationService.GetResourceAsync("Account.VendorInfo"),
                    Tab = (int)CustomerNavigationEnum.VendorInfo,
                    ItemClass = "customer-vendor-info"
                });
            }
            if (_gdprSettings.GdprEnabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "GdprTools",
                    Title = await _localizationService.GetResourceAsync("Account.Gdpr"),
                    Tab = (int)CustomerNavigationEnum.GdprTools,
                    ItemClass = "customer-gdpr"
                });
            }

            if (_captchaSettings.Enabled && _customerSettings.AllowCustomersToCheckGiftCardBalance)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CheckGiftCardBalance",
                    Title = await _localizationService.GetResourceAsync("CheckGiftCardBalance"),
                    Tab = (int)CustomerNavigationEnum.CheckGiftCardBalance,
                    ItemClass = "customer-check-gift-card-balance"
                });
            }

            if (await _permissionService.AuthorizeAsync(StandardPermissionProvider.EnableMultiFactorAuthentication) &&
                await _multiFactorAuthenticationPluginManager.HasActivePluginsAsync())
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "MultiFactorAuthenticationSettings",
                    Title = await _localizationService.GetResourceAsync("PageTitle.MultiFactorAuthentication"),
                    Tab = (int)CustomerNavigationEnum.MultiFactorAuthentication,
                    ItemClass = "customer-multiFactor-authentication"
                });
            }

            model.SelectedTab = selectedTabId;

            return model;
        }

        /// <summary>
        /// Prepare the customer address list model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer address list model
        /// </returns>
        public virtual async Task<CustomerAddressListModel> PrepareCustomerAddressListModelAsync()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            var addresses = await (await _customerService.GetAddressesByCustomerIdAsync(customer.Id))
                //enabled for the current store
                .WhereAwait(async a => a.CountryId == null || await _storeMappingService.AuthorizeAsync(await _countryService.GetCountryByAddressAsync(a)))
                .ToListAsync();

            var model = new CustomerAddressListModel();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                await _addressModelFactory.PrepareAddressModelAsync(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    loadCountries: async () => await _countryService.GetAllCountriesAsync((await _workContext.GetWorkingLanguageAsync()).Id));
                model.Addresses.Add(addressModel);
            }
            return model;
        }

        /// <summary>
        /// Prepare the customer downloadable products model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer downloadable products model
        /// </returns>
        public virtual async Task<CustomerDownloadableProductsModel> PrepareCustomerDownloadableProductsModelAsync()
        {
            var model = new CustomerDownloadableProductsModel();
            var customer = await _workContext.GetCurrentCustomerAsync();
            var items = await _orderService.GetDownloadableOrderItemsAsync(customer.Id);
            foreach (var item in items)
            {
                var order = await _orderService.GetOrderByIdAsync(item.OrderId);
                var product = await _productService.GetProductByIdAsync(item.ProductId);

                var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel
                {
                    OrderItemGuid = item.OrderItemGuid,
                    OrderId = order.Id,
                    CustomOrderNumber = order.CustomOrderNumber,
                    CreatedOn = await _dateTimeHelper.ConvertToUserTimeAsync(order.CreatedOnUtc, DateTimeKind.Utc),
                    ProductName = await _localizationService.GetLocalizedAsync(product, x => x.Name),
                    ProductSeName = await _urlRecordService.GetSeNameAsync(product),
                    ProductAttributes = item.AttributeDescription,
                    ProductId = item.ProductId
                };
                model.Items.Add(itemModel);

                if (await _orderService.IsDownloadAllowedAsync(item))
                    itemModel.DownloadId = product.DownloadId;

                if (await _orderService.IsLicenseDownloadAllowedAsync(item))
                    itemModel.LicenseId = item.LicenseDownloadId ?? 0;
            }

            return model;
        }

        /// <summary>
        /// Prepare the user agreement model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <param name="product">Product</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the user agreement model
        /// </returns>
        public virtual Task<UserAgreementModel> PrepareUserAgreementModelAsync(OrderItem orderItem, Product product)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new UserAgreementModel
            {
                UserAgreementText = product.UserAgreementText,
                OrderItemGuid = orderItem.OrderItemGuid
            };

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the change password model
        /// </returns>
        public virtual Task<ChangePasswordModel> PrepareChangePasswordModelAsync()
        {
            var model = new ChangePasswordModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the customer avatar model
        /// </summary>
        /// <param name="model">Customer avatar model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the customer avatar model
        /// </returns>
        public virtual async Task<CustomerAvatarModel> PrepareCustomerAvatarModelAsync(CustomerAvatarModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvatarUrl = await _pictureService.GetPictureUrlAsync(
                await _genericAttributeService.GetAttributeAsync<int>(await _workContext.GetCurrentCustomerAsync(), NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                false);

            return model;
        }

        /// <summary>
        /// Prepare the GDPR tools model
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the gDPR tools model
        /// </returns>
        public virtual Task<GdprToolsModel> PrepareGdprToolsModelAsync()
        {
            var model = new GdprToolsModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the check gift card balance madel
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the check gift card balance madel
        /// </returns>
        public virtual Task<CheckGiftCardBalanceModel> PrepareCheckGiftCardBalanceModelAsync()
        {
            var model = new CheckGiftCardBalanceModel();

            return Task.FromResult(model);
        }

        /// <summary>
        /// Prepare the multi-factor authentication model
        /// </summary>
        /// <param name="model">Multi-factor authentication model</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the multi-factor authentication model
        /// </returns>
        public virtual async Task<MultiFactorAuthenticationModel> PrepareMultiFactorAuthenticationModelAsync(MultiFactorAuthenticationModel model)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            model.IsEnabled = !string.IsNullOrEmpty(
                await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute));

            var store = await _storeContext.GetCurrentStoreAsync();
            var multiFactorAuthenticationProviders = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(customer, store.Id)).ToList();
            foreach (var multiFactorAuthenticationProvider in multiFactorAuthenticationProviders)
            {
                var providerModel = new MultiFactorAuthenticationProviderModel();
                var sysName = multiFactorAuthenticationProvider.PluginDescriptor.SystemName;
                providerModel = await PrepareMultiFactorAuthenticationProviderModelAsync(providerModel, sysName);
                model.Providers.Add(providerModel);
            }

            return model;
        }

        /// <summary>
        /// Prepare the multi-factor authentication provider model
        /// </summary>
        /// <param name="providerModel">Multi-factor authentication provider model</param>
        /// <param name="sysName">Multi-factor authentication provider system name</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the multi-factor authentication model
        /// </returns>
        public virtual async Task<MultiFactorAuthenticationProviderModel> PrepareMultiFactorAuthenticationProviderModelAsync(MultiFactorAuthenticationProviderModel providerModel, string sysName, bool isLogin = false)
        {
            var customer = await _workContext.GetCurrentCustomerAsync();
            var selectedProvider = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.SelectedMultiFactorAuthenticationProviderAttribute);
            var store = await _storeContext.GetCurrentStoreAsync();

            var multiFactorAuthenticationProvider = (await _multiFactorAuthenticationPluginManager.LoadActivePluginsAsync(customer, store.Id))
                    .FirstOrDefault(provider => provider.PluginDescriptor.SystemName == sysName);

            if (multiFactorAuthenticationProvider != null)
            {
                providerModel.Name = await _localizationService.GetLocalizedFriendlyNameAsync(multiFactorAuthenticationProvider, (await _workContext.GetWorkingLanguageAsync()).Id);
                providerModel.SystemName = sysName;
                providerModel.Description = await multiFactorAuthenticationProvider.GetDescriptionAsync();
                providerModel.LogoUrl = await _multiFactorAuthenticationPluginManager.GetPluginLogoUrlAsync(multiFactorAuthenticationProvider);
                providerModel.ViewComponent = isLogin ? multiFactorAuthenticationProvider.GetVerificationViewComponent() : multiFactorAuthenticationProvider.GetPublicViewComponent();
                providerModel.Selected = sysName == selectedProvider;
            }

            return providerModel;
        }

        /// <summary>
        /// Prepare the custom customer attribute models
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="overrideAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the list of the customer attribute model
        /// </returns>
        public virtual async Task<IList<CustomerAttributeModel>> PrepareCustomCustomerAttributesAsync(Customer customer, string overrideAttributesXml = "")
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var result = new List<CustomerAttributeModel>();

            var customerAttributes = await _customerAttributeService.GetAllCustomerAttributesAsync();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = await _localizationService.GetLocalizedAsync(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = await _customerAttributeService.GetCustomerAttributeValuesAsync(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = await _localizationService.GetLocalizedAsync(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                //set already selected attributes
                var selectedAttributesXml = !string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    customer.CustomCustomerAttributesXML;
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                if (!_customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id).Any())
                                    break;

                                //clear default selection                                
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = await _customerAttributeParser.ParseCustomerAttributeValuesAsync(selectedAttributesXml);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                var enteredText = _customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                result.Add(attributeModel);
            }

            return result;
        }

        public virtual async Task<TransactionModel> PrepareTransactionModelAsync(Customer customer, TransactionModel model = null)
        {
            model ??= new TransactionModel();
            if (customer is null)
                return model;

            var transactions = await _transactionService.GetAllTransactionsAsync(customerId: customer.Id,
                orderBy: 2);
            model.RecentTransactions = await transactions
                .Take(3)
                .SelectAwait(async x => new TransactionModel()
                {
                    TransactionNote = x.TransactionNote,
                    TransactionAmount = x.TransactionAmount,
                    CreateOnUtc = x.CreatedOnUtc,
                    FormattedTransactionAmount = await _priceFormatter.FormatPriceInCurrencyAsync(x.TransactionAmount),
                })
                .ToListAsync();

            model.CurrencySymbol = await _priceFormatter.GetCurrentSymbolAsync();

            return model;
        }

        public virtual async Task<WithdrawModel> PrepareWithdawModelModelAsync(Customer customer, WithdrawModel model = null)
        {
            model ??= new WithdrawModel();
            if (customer is null)
                return model;

            var transactions = await _transactionService.GetAllTransactionsAsync(customerId: customer.Id,
               orderBy: 2);
            model.RecentTransactions = await transactions
                .Take(3)
                .SelectAwait(async x => new TransactionModel()
                {
                    TransactionNote = x.TransactionNote,
                    TransactionAmount = x.TransactionAmount,
                    CreateOnUtc = x.CreatedOnUtc,
                    FormattedTransactionAmount = await _priceFormatter.FormatPriceInCurrencyAsync(x.TransactionAmount),
                })
                .ToListAsync();

            foreach (WalletTypeEnum enumValues in Enum.GetValues(typeof(WalletTypeEnum)))
            {
                model.AvailableWalletType.Add(new SelectListItem
                {
                    Text = await _localizationService.GetLocalizedEnumAsync(enumValues),
                    Value = $"{(int)enumValues}",
                });
            }

            await this.PrepareWithdrawalMethodCustomerInfoModelAsync(model.WithdrawalMethodModel, customer);

            model.CurrencySymbol = await _priceFormatter.GetCurrentSymbolAsync();

            return model;
        }

        public virtual async Task<AnalyticsModel> PrepareAnalyticsModelAsync(Customer customer)
        {
            var transactions = (await _transactionService.GetAllTransactionsAsync(customerId: customer.Id,
                transactionNote: "Return"))
                .Where(x => !x.Status.Equals(Status.Removed));

            var returnTransactions = await _transactionService.GetAllReturnTransactionsAsync(customerId: customer.Id);

            //var groupedTransactions = transactions
            //    .GroupBy(x => x.CreatedOnUtc.Year)
            //    .Select(group => new
            //    {
            //        Year = group.Key,
            //        Transactions = group.ToList(),
            //        ReturnAverage = group.Any()
            //            ? group.Average(x => x.ReturnPercentage.GetValueOrDefault())
            //            : decimal.Zero
            //    })
            //    .ToList();

            //var a = groupedTransactions
            //    .Select(item => new
            //    {
            //        name = item.Year,
            //        y = item.ReturnAverage,
            //        drilldown = item.Year
            //    })
            //    .ToList();

            var startYear = returnTransactions.Any()
                ? returnTransactions.Min(x => x.ReturnDateOnUtc.Year)
                : DateTime.Now.Year;
            var groupedReturnTransactions = await Enumerable.Range(startYear, DateTime.Now.Year - startYear + 1)
                .SelectAwait(async year =>
                {
                    var group1 = returnTransactions
                         .Where(a => a.ReturnDateOnUtc.Year.Equals(year))
                         //make a pair of year
                         .GroupBy(x => x.ReturnDateOnUtc.Year)
                         .FirstOrDefault();
                    return new
                    {
                        name = year,
                        data = await Enumerable.Range(1, 12)
                             .SelectAwait(async month =>
                             {
                                 //get the data of the each month and if any month does not have any data then pass empty values
                                 var group2 = group1
                                     ?.Where(y => y.ReturnDateOnUtc.Month.Equals(month))
                                     //make a pair of month
                                     .GroupBy(y => y.ReturnDateOnUtc.Month)
                                     .FirstOrDefault();

                                 return new object[]
                                 {
                                    //month
                                     new DateTime(year, group2?.Key ?? month ,1).ToString("MMMM"),
                                    //month average
                                    group2?.Any() ?? false
                                        ? group2.Average(y => y.ReturnPercentage)
                                        : decimal.Zero,
                                    //sum of amount in that month
                                    await _priceFormatter.FormatPriceInCurrencyAsync(group2?.Sum(y => y.ReturnAmount) ?? decimal.Zero, false)
                                 };
                             }).ToListAsync()
                    };
                })
                .OrderByDescending(x => x.name)
                .ToListAsync();
            //returnTransactions
            ////make a pair of year
            //.GroupBy(x => x.ReturnDateOnUtc.Year)
            //.Select(group1 => new
            //{
            //    name = group1.Key,
            //    data = Enumerable.Range(1, 12)
            //            .Select(month =>
            //            {
            //                //get the data of the each month and if any month does not have any data then pass empty values
            //                var group2 = group1
            //                    .Where(y => y.ReturnDateOnUtc.Month.Equals(month))
            //                    .GroupBy(y => y.ReturnDateOnUtc.Month).FirstOrDefault();

            //                return new object[]
            //                {
            //                    //month
            //                     new DateTime(group1.Key, group2?.Key ?? month ,1).ToString("MMMM"),
            //                    //month average
            //                    group2?.Any() ?? false
            //                        ? group2.Average(y => y.ReturnPercentage)
            //                        : decimal.Zero,
            //                    //sum of amount in that month
            //                    group2?.Sum(y => y.ReturnAmount) ?? decimal.Zero
            //                };
            //            })
            //})
            //.ToList();

            //var groupedTransactions = transactions
            //    //make a pair of year
            //    .GroupBy(x => x.CreatedOnUtc.Year)
            //    .Select(group1 => new
            //    {
            //        name = group1.Key,
            //                //send data of every month even if any month does not have any data but if in case pass empty values like zero
            //        data = Enumerable.Range(1, 12)
            //                .Select(month =>
            //                {
            //                    var previousMonth = (month - 1 + 12) % 12 + 1; // Calculate the previous month

            //                    //get the data of the each month and if any month does not have any data then pass empty values
            //                    var group2 = group1
            //                        .Where(y => y.CreatedOnUtc.Month.Equals(month))
            //                        .GroupBy(y => y.CreatedOnUtc.Month).FirstOrDefault();

            //                    return new object[]
            //                    {
            //                        //month
            //                         new DateTime(group1.Key, group2?.Key ?? month ,1).ToString("MMMM"),
            //                        //month average
            //                        group2?.Any() ?? false
            //                            ? group2.Average(y => y.ReturnPercentage.GetValueOrDefault())
            //                            : decimal.Zero,
            //                        //sum of amount in that month
            //                        group2?.Sum(y => y.TransactionAmount) ?? decimal.Zero
            //                    };
            //                })
            //    })
            //    .ToList();

            var model = new AnalyticsModel()
            {
                //Year = JsonConvert.SerializeObject(a),
                MonthsAndGain = JsonConvert.SerializeObject(new
                {
                    groupedReturnTransactions = groupedReturnTransactions,
                    currenySymbol = await _priceFormatter.GetCurrentSymbolAsync()
                })
            };

            return model;
        }

        public virtual async Task PrepareWithdrawalMethodCustomerInfoModelAsync(IList<WithdrawalMethodCustomerInfoModel> model, Customer customer)
        {
            foreach (WalletTypeEnum enumValues in Enum.GetValues(typeof(WalletTypeEnum)))
            {
                var tempModel = new WithdrawalMethodCustomerInfoModel()
                {
                    Id = (int)enumValues,
                    Name = await _localizationService.GetLocalizedEnumAsync(enumValues),
                };

                var withdrawalMethods = await _withdrawService.GetAllWithdrawalMethodAsync(typeId: (int)enumValues,
                    isEnabled: true);
                if (!withdrawalMethods.Any())
                    continue;

                foreach (var withdrawalMethod in withdrawalMethods)
                {
                    var tempModel1 = new WithdrawalMethodCustomerInfoModel()
                    {
                        Id = withdrawalMethod.Id,
                        Name = withdrawalMethod.Name,
                    };

                    var withdrawalMethodFields = await _withdrawService.GetAllWithdrawalMethodFieldAsync(withdrawalMethodId: withdrawalMethod.Id,
                        isEnabled: true);
                    if (!withdrawalMethodFields.Any())
                        continue;

                    foreach (var withdrawalMethodField in withdrawalMethodFields)
                    {
                        var customerWithdrawalMethod = (await _withdrawService.GetAllCustomerWithdrawalMethodAsync(customerId: customer.Id,
                            withdrawalMethodFieldId: withdrawalMethodField.Id))
                            .FirstOrDefault();

                        tempModel1.Fields.Add(new WithdrawalMethodCustomerInfoModel()
                        {
                            Id = withdrawalMethodField.Id,
                            Name = withdrawalMethodField.FieldName,
                            Value = customerWithdrawalMethod?.Value
                        });
                    }

                    tempModel.Fields.Add(tempModel1);
                }

                model.Add(tempModel);
            }
        }


        #endregion
    }
}