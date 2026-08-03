using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.IServices
{
    public interface ICategoryService
    {
        Task<IReadOnlyCollection<CategoryShortDto>> GetAllAsync();
    }
}