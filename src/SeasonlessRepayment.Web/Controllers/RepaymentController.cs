using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SeasonlessRepayment.Domain;
using SeasonlessRepayment.Services;

namespace SeasonlessRepayment.Web.Controllers
{
    public class RepaymentController : BaseApiController
    {
        private readonly IRepaymentService _repaymentService;

        public RepaymentController(IRepaymentService repaymentService)
        {
            _repaymentService = repaymentService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var repayments = await _repaymentService.List();
            return BaseApiSuccessResult(new
                {repayments = repayments.Select(r => new {r.Id, r.Amount, r.Customer, r.Season, r.ParentId, r.Date})});
        }


        [HttpPost]
        [Route("Upload")]
        public async Task<IActionResult> Upload(List<Models.RepaymentUpload> uploads)
        {
            var repaymentUploads = uploads.Select(u => new RepaymentUpload
            {
                Amount = u.Amount,
                CustomerId = u.CustomerId,
                Date = DateTime.ParseExact(u.Date, "d/M/yyyy", CultureInfo.CurrentUICulture),
                SeasonId = u.SeasonId
            });
            var repayments = await _repaymentService.ProcessUploads(repaymentUploads);

            return BaseApiSuccessResult(new
                {repayments = repayments.Select(r => new {r.Amount, r.CustomerId, r.SeasonId, r.ParentId, r.Date})});
        }
    }
}