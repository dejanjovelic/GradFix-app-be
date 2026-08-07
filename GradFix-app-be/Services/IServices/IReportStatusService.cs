using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.IServices
{
    public interface IReportStatusService
    {
        Task<IReadOnlyCollection<ReportStatusShortDto>> GetAllAsync();
        Task<ReportStatusShortDto> GetByNameAsync(string statusName);
    }
}