using AutoMapper;
using GradFix_app_be.Domain.IRepositories;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.IServices;

namespace GradFix_app_be.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        public async Task<IReadOnlyCollection<CategoryShortDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();

            return _mapper.Map<List<CategoryShortDto>>(categories);
        }
    }
}
}
