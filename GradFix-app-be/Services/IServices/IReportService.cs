using System.Threading.Tasks;
using GradFix_app_be.Services.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace GradFix_app_be.Services.IServices
{
    public interface IReportService
    {
        Task<ReportResponseDto> CreateReportAsync(ReportCreateDto dto, string? reporterId);
        Task<ReportResponseDto> GetByIdAsync(int id);
    }
}
