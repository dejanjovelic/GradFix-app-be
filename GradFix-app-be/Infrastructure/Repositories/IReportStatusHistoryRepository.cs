using GradFix_app_be.Domain;

namespace GradFix_app_be.Infrastructure.Repositories
{
    public interface IReportStatusHistoryRepository
    {
        Task AddAsync(ReportStatusHistory statusHistory);
    }
}