using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeasonlessRepayment.Services;

namespace SeasonlessRepayment.Web.Controllers
{
    public class SeasonController : BaseApiController
    {
        private readonly ISeasonService _seasonService;

        public SeasonController(ISeasonService seasonService)
        {
            _seasonService = seasonService;
        }

        [HttpPost]
        [Route("{name}")]
        public async Task<IActionResult> Create([FromRoute] string name)
        {
            await _seasonService.Create(name);
            return BaseApiSuccessResult();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var seasons = await _seasonService.GetAll();
            return BaseApiSuccessResult(new {seasons});
        }
    }
}