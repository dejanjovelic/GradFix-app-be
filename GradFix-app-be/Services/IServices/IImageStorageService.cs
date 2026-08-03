using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.IServices
{
    public interface IImageStorageService
    {
        Task<IReadOnlyList<StoredImageDto>> SaveReportImagesAsync( IReadOnlyCollection<IFormFile> images);
    }
}
