using GradFix_app_be.Domain;

namespace GradFix_app_be.Domain.IRepositories
{
    public interface IReportStatusRepository
    {
        Task<ReportStatus> GetByNameAsync(string statusName);
        Task<List<ReportStatus>> GetAllAsync();
    }
}