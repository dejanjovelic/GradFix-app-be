using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using GradFix_app_be.Services;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] ReportCreateDto dto)
        {
            // reporter id from authenticated user
            var reporterId = User?.Identity?.IsAuthenticated == true ? User.FindFirst("sub")?.Value ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value : null;

            var id = await _reportService.CreateReportAsync(dto, reporterId);
            return CreatedAtAction(nameof(GetById), new { id }, new { id });
        }

        [HttpGet("{id}")]
        public IActionResult GetById(System.Guid id)
        {
            // Placeholder until read methods are implemented
            return NotFound();
        }
    }
}
