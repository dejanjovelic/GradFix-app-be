using GradFix_app_be.Domain;
using GradFix_app_be.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GradFix_app_be.Infrastructure.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _dbContext;

        public ReportRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Report> AddAsync(Report report)
        {
            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();
            return report;
        }

        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _dbContext.Reports
                .Include(r => r.Category)
                .Include(r => r.Reporter)
                .Include(r => r.Status)
                .Include(r => r.Images)
                .Include(r => r.StatusHistory)
                    .ThenInclude(h => h.OldStatus)
                .Include(r => r.StatusHistory)
                    .ThenInclude(h => h.NewStatus)
                .Include(r => r.StatusHistory)
                    .ThenInclude(h => h.ChangedByUser)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<PaginatedList<Report>> GetAllAsync(
                int page,
                int pageSize,
                int? categoryId = null,
                int? statusId = null)
        {
            var query = _dbContext.Reports
                .AsNoTracking()
                .Include(report => report.Category)
                .Include(report => report.Status)
                .Include(report => report.Images)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(report => report.CategoryId == categoryId.Value);
            }

            if (statusId.HasValue)
            {
                query = query.Where(report => report.StatusId == statusId.Value);
            }

            var totalRowCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(report => report.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<Report>(items, page, pageSize, totalRowCount);
        }

        public async Task<List<Report>> GetMapItemsAsync(int? categoryId = null, int? statusId = null)
        {
            var query = _dbContext.Reports
                .AsNoTracking()
                .Where(report =>
                    report.Latitude.HasValue &&
                    report.Longitude.HasValue)
                .Include(report => report.Category)
                .Include(report => report.Status)
                .Include(report => report.Images)
                .AsSplitQuery()
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(report => report.CategoryId == categoryId.Value);
            }

            if (statusId.HasValue)
            {
                query = query.Where(report => report.StatusId == statusId.Value);
            }

            return await query
                .OrderByDescending(report => report.CreatedAt)
                .ToListAsync();
        }
    }
}
