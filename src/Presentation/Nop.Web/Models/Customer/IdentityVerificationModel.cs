using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Web.Models.Customer
{
    public partial record IdentityVerificationModel : BaseNopEntityModel
    {

        [NopResourceDisplayName("Account.ChangePassword.Fields.File")]
        public IFormFile FormId { get; set; }

        [NopResourceDisplayName("Account.ChangePassword.Fields.ProofOfAddress")]
        public IFormFile ProofOfAddress { get; set; }

        [NopResourceDisplayName("Account.ChangePassword.Fields.Document")]
        public IFormFile Document { get; set; }
    }
}
