using GradFix_app_be.Services.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GradFix_app_be.Services.IServices
{
    public interface IReportService
    {
        Task<ReportResponseDto> CreateAsync(ReportCreateDto dto, string? reporterId);
        Task<ReportResponseDto> GetByIdAsync(int id);
        Task<PaginatedListDto<ReportListItemDto>> GetAllAsync(ReportQueryDto query);
        Task<IReadOnlyCollection<ReportListItemDto>> GetMapItemsAsync(int? categoryId, int? statusId);
        Task<ReportResponseDto> UpdateStatusAsync(int reportId, ReportStatusUpdateDto dto, string? changedByUserId);
        Task<PaginatedListDto<ReportListItemDto>> GetMineAsync(ReportQueryDto query, string? reporterId);
    }
}
