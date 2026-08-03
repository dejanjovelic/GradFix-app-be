using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Exceptions;
using GradFix_app_be.Services.IServices;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
namespace GradFix_app_be.Services
{
    public class ImageStorageService : IImageStorageService
    {
        private const int MaxImageCount = 3;
        private const long MaxInputFileSize = 10 * 1024 * 1024;
        private const int MaxImageWidth = 1600;
        private const int MaxImageHeight = 1600;
        private const int JpegQuality = 80;

        private static readonly HashSet<string> AllowedContentTypes =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

        private readonly IWebHostEnvironment _environment;

        public ImageStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<IReadOnlyList<StoredImageDto>> SaveReportImagesAsync(IReadOnlyCollection<IFormFile> images)
        {
            if (images.Count is < 1 or > MaxImageCount)
            {
                throw new BadRequestException(
                    "A report must contain between 1 and 3 images.");
            }

            var currentDate = DateTime.UtcNow;

            var relativeDirectory = Path.Combine("uploads", "reports", currentDate.Year.ToString(), currentDate.Month.ToString("00"));
            var webRootPath = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
            var physicalDirectory = Path.Combine(webRootPath, relativeDirectory);

            Directory.CreateDirectory(physicalDirectory);

            var storedImages = new List<StoredImageDto>();

            var order = 0;

            foreach (var file in images)
            {
                ValidateFile(file);

                try
                {
                    await using var inputStream = file.OpenReadStream();

                    using var image = await Image.LoadAsync(inputStream);

                    image.Mutate(context =>
                    {
                        context.AutoOrient();

                        context.Resize(new ResizeOptions
                        {
                            Mode = ResizeMode.Max,
                            Size = new Size(
                                    MaxImageWidth,
                                    MaxImageHeight)
                        });
                    });

                    var storedFileName = $"{Guid.NewGuid():N}.jpg";

                    var physicalPath = Path.Combine(physicalDirectory, storedFileName);

                    await image.SaveAsJpegAsync(physicalPath, new JpegEncoder
                    {
                        Quality = JpegQuality
                    });

                    var fileInfo = new FileInfo(physicalPath);

                    var publicPath = "/" + Path.Combine( relativeDirectory, storedFileName)
                            .Replace('\\', '/');

                    storedImages.Add(
                        new StoredImageDto
                        {
                            FileName = storedFileName,
                            FilePath = publicPath,
                            ContentType = "image/jpeg",
                            Size = checked(
                                (int)fileInfo.Length),
                            Order = order++
                        });
                }
                catch (UnknownImageFormatException)
                {
                    throw new BadRequestException(
                        $"File '{file.FileName}' is not a valid image.");
                }
            }

            return storedImages;
        }

        private static void ValidateFile(IFormFile file)
        {
            if (file.Length <= 0)
            {
                throw new BadRequestException(
                    $"File '{file.FileName}' is empty.");
            }

            if (file.Length > MaxInputFileSize)
            {
                throw new BadRequestException(
                    $"File '{file.FileName}' exceeds the 10 MB limit.");
            }

            if (!AllowedContentTypes.Contains(file.ContentType))
            {
                throw new BadRequestException(
                    $"File type '{file.ContentType}' is not supported.");
            }
        }

    }
}
