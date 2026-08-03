using GradFix_app_be.Domain;

namespace GradFix_app_be.Domain.IRepositories
{
    public interface ICategoryRepository
    {
        Task<bool> ExistsAsync(int categoryId);
        Task<List<Category>> GetAllAsync();
    }
}