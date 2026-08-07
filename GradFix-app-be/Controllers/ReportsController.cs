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

        //POST api/reports
        [HttpPost]
        [Authorize(Roles = "Citizen")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] ReportCreateDto dto)
        {
            var reporterId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ReportResponseDto report = await _reportService.CreateReportAsync(dto, reporterId);
            return CreatedAtAction(
                    nameof(GetById),
                    new { id = report.Id },
                    report);
        }

        //GET api/reports/1
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            ReportResponseDto report = await _reportService.GetByIdAsync(id);
            return Ok(report);
        }

        //GET api/reports
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] ReportQueryDto query)
        {
            var reports = await _reportService.GetAllAsync(query);

            return Ok(reports);
        }

        //GET api/reports/map
        [HttpGet("map")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMapItems([FromQuery] int? categoryId, [FromQuery] int? statusId)
        {
            var reports = await _reportService.GetMapItemsAsync( categoryId, statusId);

            return Ok(reports);
        }
    }
}
