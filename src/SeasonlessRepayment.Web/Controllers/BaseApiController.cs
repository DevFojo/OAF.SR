using Microsoft.AspNetCore.Mvc;

namespace SeasonlessRepayment.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        [NonAction]
        protected IActionResult BaseApiSuccessResult(object data = null)
        {
            return StatusCode(200, new BaseApiResponse(true, data));
        }

        public class BaseApiResponse
        {
            public BaseApiResponse(bool success, object data = null, string error = null)
            {
                Success = success;
                Error = error;
                Data = data ?? new { };
            }

            public object Data { get; set; }
            public bool Success { get; set; }
            public string Error { get; set; }
        }
    }
}