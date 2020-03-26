using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeasonlessRepayment.Services;

namespace SeasonlessRepayment.Web.Controllers
{
    public class CustomerController : BaseApiController
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var customers = await _customerService.GetAll();
            return BaseApiSuccessResult(new {customers});
        }
    }
}