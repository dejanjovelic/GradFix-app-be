using GradFix_app_be.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GradFix_app_be.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportStatusesController : ControllerBase
    {
        private readonly IReportStatusService
            _reportStatusService;

        public ReportStatusesController(IReportStatusService reportStatusService)
        {
            _reportStatusService = reportStatusService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var statuses =
                await _reportStatusService
                    .GetAllAsync();

            return Ok(statuses);
        }

    }
}
