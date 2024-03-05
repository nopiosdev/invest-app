using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Models.Common;

namespace Nop.Web.Models.Customer
{
    public partial record CustomerInfoModel : BaseNopModel
    {
        public CustomerInfoModel()
        {
            AvailableTimeZones = new List<SelectListItem>();
            AvailableCountries = new List<SelectListItem>();
            AvailableStates = new List<SelectListItem>();
            AssociatedExternalAuthRecords = new List<AssociatedExternalAuthModel>();
            CustomerAttributes = new List<CustomerAttributeModel>();
            GdprConsents = new List<GdprConsentModel>();

            AvailableGoal = new List<SelectListItem>();
            AvailableTimeline = new List<SelectListItem>();
            AvailableExperience = new List<SelectListItem>();
            AvailableRiskTolerance = new List<SelectListItem>();
            AvailableInvestmentApproach = new List<SelectListItem>();
            AvailablePaymentType = new List<SelectListItem>();

            WithdrawalMethodModel = new List<WithdrawalMethodCustomerInfoModel>();
            Address = new AddressModel();
        }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Account.Fields.Email")]
        public string Email { get; set; }
        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Account.Fields.EmailToRevalidate")]
        public string EmailToRevalidate { get; set; }

        public bool CheckUsernameAvailabilityEnabled { get; set; }
        public bool AllowUsersToChangeUsernames { get; set; }
        public bool UsernamesEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Username")]
        public string Username { get; set; }

        //form fields & properties
        public bool GenderEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Gender")]
        public string Gender { get; set; }

        public bool NeutralGenderEnabled { get; set; }

        public bool FirstNameEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.FirstName")]
        public string FirstName { get; set; }
        public bool FirstNameRequired { get; set; }
        public bool LastNameEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.LastName")]
        public string LastName { get; set; }
        public bool LastNameRequired { get; set; }

        public bool DateOfBirthEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthDay { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthMonth { get; set; }
        [NopResourceDisplayName("Account.Fields.DateOfBirth")]
        public int? DateOfBirthYear { get; set; }
        public bool DateOfBirthRequired { get; set; }
        public DateTime? ParseDateOfBirth()
        {
            return CommonHelper.ParseDate(DateOfBirthYear, DateOfBirthMonth, DateOfBirthDay);
        }

        public bool CompanyEnabled { get; set; }
        public bool CompanyRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.Company")]
        public string Company { get; set; }
        public bool StreetAddressEnabled { get; set; }
        public bool StreetAddressRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.StreetAddress")]
        public string StreetAddress { get; set; }

        public bool StreetAddress2Enabled { get; set; }
        public bool StreetAddress2Required { get; set; }
        [NopResourceDisplayName("Account.Fields.StreetAddress2")]
        public string StreetAddress2 { get; set; }

        public bool ZipPostalCodeEnabled { get; set; }
        public bool ZipPostalCodeRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.ZipPostalCode")]
        public string ZipPostalCode { get; set; }

        public bool CityEnabled { get; set; }
        public bool CityRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.City")]
        public string City { get; set; }

        public bool CountyEnabled { get; set; }
        public bool CountyRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.County")]
        public string County { get; set; }

        public bool CountryEnabled { get; set; }
        public bool CountryRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.Country")]
        public int CountryId { get; set; }
        public IList<SelectListItem> AvailableCountries { get; set; }

        public bool StateProvinceEnabled { get; set; }
        public bool StateProvinceRequired { get; set; }
        [NopResourceDisplayName("Account.Fields.StateProvince")]
        public int StateProvinceId { get; set; }
        public IList<SelectListItem> AvailableStates { get; set; }

        public bool PhoneEnabled { get; set; }
        public bool PhoneRequired { get; set; }
        [DataType(DataType.PhoneNumber)]
        [NopResourceDisplayName("Account.Fields.Phone")]
        public string Phone { get; set; }

        public bool FaxEnabled { get; set; }
        public bool FaxRequired { get; set; }
        [DataType(DataType.PhoneNumber)]
        [NopResourceDisplayName("Account.Fields.Fax")]
        public string Fax { get; set; }

        public bool NewsletterEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Newsletter")]
        public bool Newsletter { get; set; }

        //preferences
        public bool SignatureEnabled { get; set; }
        [NopResourceDisplayName("Account.Fields.Signature")]
        public string Signature { get; set; }

        //time zone
        [NopResourceDisplayName("Account.Fields.TimeZone")]
        public string TimeZoneId { get; set; }
        public bool AllowCustomersToSetTimeZone { get; set; }
        public IList<SelectListItem> AvailableTimeZones { get; set; }

        //EU VAT
        [NopResourceDisplayName("Account.Fields.VatNumber")]
        public string VatNumber { get; set; }
        public string VatNumberStatusNote { get; set; }
        public bool DisplayVatNumber { get; set; }

        //external authentication
        [NopResourceDisplayName("Account.AssociatedExternalAuth")]
        public IList<AssociatedExternalAuthModel> AssociatedExternalAuthRecords { get; set; }
        public int NumberOfExternalAuthenticationProviders { get; set; }
        public bool AllowCustomersToRemoveAssociations { get; set; }

        public IList<CustomerAttributeModel> CustomerAttributes { get; set; }

        public IList<GdprConsentModel> GdprConsents { get; set; }


        [NopResourceDisplayName("Account.Goal")]
        public int GoalId { get; set; }
        public IList<SelectListItem> AvailableGoal { get; set; }

        [NopResourceDisplayName("Account.Timeline")]
        public int TimelineId { get; set; }
        public IList<SelectListItem> AvailableTimeline { get; set; }

        [NopResourceDisplayName("Account.Experience")]
        public int ExperienceId { get; set; }
        public IList<SelectListItem> AvailableExperience { get; set; }

        [NopResourceDisplayName("Account.RiskTolerance")]
        public int RiskToleranceId { get; set; }
        public IList<SelectListItem> AvailableRiskTolerance { get; set; }

        [NopResourceDisplayName("Account.InvestmentApproach")]
        public int InvestmentApproachId { get; set; }
        public IList<SelectListItem> AvailableInvestmentApproach { get; set; }

        [NopResourceDisplayName("Account.TwoFactorAuthentication")]
        public bool TwoFactorAuthentication { get; set; }

        [NopResourceDisplayName("Account.EmailAlert")]
        public bool EmailAlert { get; set; }

        [NopResourceDisplayName("Account.TextAlert")]
        public bool TextAlert { get; set; }

        [NopResourceDisplayName("Account.PaymentType")]
        public int PaymentType { get; set; }
        public IList<SelectListItem> AvailablePaymentType { get; set; }

        [NopResourceDisplayName("account.document.Fields.FormId")]
        public IFormFile FormId { get; set; }

        [NopResourceDisplayName("account.document.Fields.ProofOfAddress")]
        public IFormFile ProofOfAddress { get; set; }

        [NopResourceDisplayName("account.document.Fields.Document")]
        public IFormFile Document { get; set; }

        public IList<WithdrawalMethodCustomerInfoModel> WithdrawalMethodModel { get; set; }
        public int DefaultWithdrawalMethodId { get; set; }

        public AddressModel Address { get; set; }
        public ChangePasswordModel ChangePasswordModel { get; set; }

        #region Nested classes

        public partial record AssociatedExternalAuthModel : BaseNopEntityModel
        {
            public string Email { get; set; }

            public string ExternalIdentifier { get; set; }

            public string AuthMethodName { get; set; }
        }


        #endregion
    }
}