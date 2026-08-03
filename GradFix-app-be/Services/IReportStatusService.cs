using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services
{
    public interface IReportStatusService
    {
        Task<IReadOnlyCollection<ReportStatusShortDto>> GetAllAsync();
        Task<ReportStatusShortDto> GetByNameAsync(string statusName);
    }
}