using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Services.Customers;

namespace Nop.Web.Controllers
{
    //[AuthorizeCustomer]
    public partial class HomeController : BasePublicController
    {
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;

        public HomeController(IWorkContext workContext, ICustomerService customerService)
        {
            _workContext = workContext;
            _customerService = customerService;
        }

        public virtual async Task<IActionResult> Index()
        {
            var customer = await _workContext.GetCurrentCustomerAsync();

            // Check if the customer is a registered user
            bool isRegistered = await _customerService.IsRegisteredAsync(customer);

            if (isRegistered)
            {
                // hide buttion to login and replace with dashboard button
            }
            else
            {
                //show button option to login
            }

            return View(isRegistered);
        }
    }
}