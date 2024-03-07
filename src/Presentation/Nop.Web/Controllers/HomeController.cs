using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Web.Controllers
{
    [AuthorizeCustomer]
    public partial class HomeController : BasePublicController
    {
        public virtual IActionResult Index()
        {
            return View();
        }
    }
}