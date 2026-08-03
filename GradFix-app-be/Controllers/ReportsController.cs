using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

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
        [Authorize(Roles = "Citizen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ReportCreateDto dto)
        {
            var reporterId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            ReportResponseDto report = await _reportService.CreateReportAsync(dto, reporterId);
            return CreatedAtAction(
                    nameof(GetByIdAsync),
                    new { id = report.Id },
                    report);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            ReportResponseDto report = await _reportService.GetByIdAsync(id);
            return Ok(report);
        }
    }
}
