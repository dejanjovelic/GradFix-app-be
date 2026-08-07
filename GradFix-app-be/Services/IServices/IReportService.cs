using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Mappings;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GradFix_app_be.Services.IServices
{
    public interface IReportService
    {
        Task<ReportResponseDto> CreateReportAsync(ReportCreateDto dto, string? reporterId);
        Task<ReportResponseDto> GetByIdAsync(int id);
        Task<PaginatedListDto<ReportListItemDto>> GetAllAsync(ReportQueryDto query);
        Task<IReadOnlyCollection<ReportMapItemDto>> GetMapItemsAsync(int? categoryId, int? statusId);
    }
}
