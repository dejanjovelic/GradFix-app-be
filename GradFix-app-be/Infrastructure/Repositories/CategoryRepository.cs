using GradFix_app_be.Domain;
using GradFix_app_be.Domain.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GradFix_app_be.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _dbContext;

        public CategoryRepository(AppDbContext context)
        {
            _dbContext = context;
        }

        public async Task<bool> ExistsAsync(int categoryId)
        {
            return await _dbContext.Categories
                .AnyAsync(category => category.Id == categoryId);
        }

        public async Task<List<Category>> GetAllAsync()
        {
            return await _dbContext.Categories.ToListAsync();
        }
    }
}
