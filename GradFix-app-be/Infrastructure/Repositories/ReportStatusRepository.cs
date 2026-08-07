using Microsoft.EntityFrameworkCore;
using GradFix_app_be.Domain;
using GradFix_app_be.Domain.IRepositories;

namespace GradFix_app_be.Infrastructure.Repositories
{
    public class ReportStatusRepository : IReportStatusRepository
    {
        private readonly AppDbContext _dbContext;

        public ReportStatusRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<ReportStatus> GetByNameAsync(string statusName)
        {
            return await _dbContext.ReportStatuses
                .FirstOrDefaultAsync(status =>
                status.Name == statusName);
        }

        public async Task<List<ReportStatus>> GetAllAsync()
        {
            return await _dbContext.ReportStatuses
                .AsNoTracking()
                .OrderBy(status => status.Id)
                .ToListAsync();
        }

        public async Task<ReportStatus?> GetByIdAsync(int id)
        {
            return await _dbContext.ReportStatuses
                .AsNoTracking()
                .FirstOrDefaultAsync(status => status.Id == id);
        }

    }
}
