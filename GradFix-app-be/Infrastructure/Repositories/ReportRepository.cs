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
    }
}
