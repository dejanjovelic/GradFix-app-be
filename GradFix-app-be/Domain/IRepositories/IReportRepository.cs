using GradFix_app_be.Domain;

namespace GradFix_app_be.Domain.IRepositories
{
    public interface IReportRepository
    {
        Task<Report> AddAsync(Report report);
        Task<Report?> GetByIdAsync(int id);
        Task<PaginatedList<Report>> GetAllAsync(int page, int pageSize, int? categoryId = null, int? statusId = null);
    }
}