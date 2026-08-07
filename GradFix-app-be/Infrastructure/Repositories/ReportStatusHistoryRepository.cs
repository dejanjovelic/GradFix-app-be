using GradFix_app_be.Domain;

namespace GradFix_app_be.Infrastructure.Repositories
{
    public class ReportStatusHistoryRepository : IReportStatusHistoryRepository
    {
        private readonly AppDbContext _dbContext;

        public ReportStatusHistoryRepository(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(
            ReportStatusHistory statusHistory)
        {
            await _dbContext.ReportStatusHistories
                .AddAsync(statusHistory);
        }
    }
}
