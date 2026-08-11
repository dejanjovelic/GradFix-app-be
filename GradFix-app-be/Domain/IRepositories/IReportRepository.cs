using GradFix_app_be.Domain;

namespace GradFix_app_be.Domain.IRepositories
{
    public interface IReportRepository
    {
        Task AddAsync(Report report);
        Task<Report?> GetByIdAsync(int id);
        Task<PaginatedList<Report>> GetAllAsync(int page, int pageSize, int? categoryId, int? statusId, string? searchQuery);
        Task<List<Report>> GetMapItemsAsync(int? categoryId, int? statusId);
        Task<Report?> GetForStatusUpdateAsync(int id);
        Task<PaginatedList<Report>> GetMineAsync(string reporterId, int page, int pageSize, int? categoryId, int? statusId, string? searchQuery);

    }
}