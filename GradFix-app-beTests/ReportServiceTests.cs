using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Domain.IRepositories;
using GradFix_app_be.Services;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Exceptions;
using GradFix_app_be.Services.IServices;
using Microsoft.AspNetCore.Http;
using System.Timers;
using Moq;

namespace GradFix_app_beTests
{
    public class ReportServiceTests
    {
        private readonly Mock<IReportRepository> _reportRepositoryMock;
        private readonly Mock<IReportStatusRepository> _reportStatusRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IImageStorageService> _imageStorageServiceMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly ReportService _service;

        public ReportServiceTests()
        {
            _reportRepositoryMock = new Mock<IReportRepository>();
            _reportStatusRepositoryMock =
                new Mock<IReportStatusRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _imageStorageServiceMock =
                new Mock<IImageStorageService>();
            _mapperMock = new Mock<IMapper>();

            _service = new ReportService(
                _reportRepositoryMock.Object,
                _reportStatusRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _imageStorageServiceMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task CreateReportAsync_WhenReporterIdIsMissing_ThrowsUnauthorizedException()
        {
            // Arrange
            var dto = CreateValidDto();

            // Act
            var action = () =>
                _service.CreateReportAsync(dto, null);

            // Assert
            var exception =
                await Assert.ThrowsAsync<UnauthorizedException>(
                    action);

            Assert.Equal(
                "Authenticated user identifier is missing.",
                exception.Message);

            _categoryRepositoryMock.Verify(
                repository => repository.ExistsAsync(
                    It.IsAny<int>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateReportAsync_WhenCategoryDoesNotExist_ThrowsBadRequestException()
        {
            // Arrange
            var dto = CreateValidDto();

            _categoryRepositoryMock
                .Setup(repository =>
                    repository.ExistsAsync(dto.CategoryId))
                .ReturnsAsync(false);

            // Act
            var action = () =>
                _service.CreateReportAsync(
                    dto,
                    "citizen-id");

            // Assert
            var exception =
                await Assert.ThrowsAsync<BadRequestException>(
                    action);

            Assert.Equal(
                "The selected category does not exist.",
                exception.Message);

            _reportStatusRepositoryMock.Verify(
                repository => repository.GetByNameAsync(
                    It.IsAny<string>()),
                Times.Never);

            _imageStorageServiceMock.Verify(
                service => service.SaveReportImagesAsync(
                    It.IsAny<List<IFormFile>>()),
                Times.Never);

            _reportRepositoryMock.Verify(
                repository => repository.AddAsync(
                    It.IsAny<Report>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateReportAsync_WhenInitialStatusDoesNotExist_ThrowsInvalidOperationException()
        {
            // Arrange
            var dto = CreateValidDto();

            _categoryRepositoryMock
                .Setup(repository =>
                    repository.ExistsAsync(dto.CategoryId))
                .ReturnsAsync(true);

            _reportStatusRepositoryMock
                .Setup(repository =>
                    repository.GetByNameAsync("New"))
                .ReturnsAsync((ReportStatus?)null);

            // Act
            var action = () =>
                _service.CreateReportAsync(
                    dto,
                    "citizen-id");

            // Assert
            var exception =
                await Assert.ThrowsAsync<InvalidOperationException>(
                    action);

            Assert.Equal(
                "The initial report status 'New' is not configured.",
                exception.Message);

            _imageStorageServiceMock.Verify(
                service => service.SaveReportImagesAsync(
                    It.IsAny<List<IFormFile>>()),
                Times.Never);

            _reportRepositoryMock.Verify(
                repository => repository.AddAsync(
                    It.IsAny<Report>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateReportAsync_WithValidData_CreatesAndReturnsReport()
        {
            // Arrange
            var dto = CreateValidDto();

            var status = new ReportStatus
            {
                Id = 1,
                Name = "New"
            };

            var storedImages = new List<StoredImageDto>
            {
                new()
                {
                    FileName = "image-1.jpg",
                    FilePath = "/uploads/reports/image-1.jpg",
                    ContentType = "image/jpeg",
                    Size = 150_000,
                    Order = 0
                }
            };

            Report? addedReport = null;

            _categoryRepositoryMock
                .Setup(repository =>
                    repository.ExistsAsync(dto.CategoryId))
                .ReturnsAsync(true);

            _reportStatusRepositoryMock
                .Setup(repository =>
                    repository.GetByNameAsync("New"))
                .ReturnsAsync(status);

            _imageStorageServiceMock
                .Setup(service =>
                    service.SaveReportImagesAsync(dto.Images))
                .ReturnsAsync(storedImages);

            _reportRepositoryMock
    .Setup(repository =>
        repository.AddAsync(It.IsAny<Report>()))
    .Callback<Report>(report =>
    {
        report.Id = 25;
        addedReport = report;
    })
    .ReturnsAsync((Report report) => report);

            _mapperMock
                .Setup(mapper =>
                    mapper.Map<ReportResponseDto>(
                        It.IsAny<Report>()))
                .Returns((Report report) =>
                    new ReportResponseDto
                    {
                        Id = report.Id,
                        Title = report.Title,
                        Description = report.Description,
                        CategoryId = report.CategoryId,
                        ReporterId = report.ReporterId,
                        StatusId = report.StatusId,
                        Latitude = report.Latitude,
                        Longitude = report.Longitude,
                        AddressFallback =
                            report.AddressFallback,
                        CreatedAt = report.CreatedAt
                    });

            // Act
            var result =
                await _service.CreateReportAsync(
                    dto,
                    "citizen-id");

            // Assert
            Assert.NotNull(addedReport);

            Assert.Equal(25, result.Id);
            Assert.Equal("Broken street light", result.Title);
            Assert.Equal(
                "The street light is not working.",
                result.Description);
            Assert.Equal(dto.CategoryId, result.CategoryId);
            Assert.Equal("citizen-id", result.ReporterId);
            Assert.Equal(status.Id, result.StatusId);

            Assert.Equal(
                "Broken street light",
                addedReport.Title);

            Assert.Equal(
                "The street light is not working.",
                addedReport.Description);

            Assert.Equal(
                "Main Street 12",
                addedReport.AddressFallback);

            Assert.Single(addedReport.Images);

            var reportImage = addedReport.Images.Single();

            Assert.Equal(
                "image-1.jpg",
                reportImage.FileName);

            Assert.Equal(
                "/uploads/reports/image-1.jpg",
                reportImage.FilePath);

            Assert.Equal(
                "image/jpeg",
                reportImage.ContentType);

            Assert.Equal(0, reportImage.Order);

            _reportRepositoryMock.Verify(
                repository =>
                    repository.AddAsync(
                        It.IsAny<Report>()),
                Times.Once);

            _mapperMock.Verify(
                mapper =>
                    mapper.Map<ReportResponseDto>(
                        It.IsAny<Report>()),
                Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenReportDoesNotExist_ThrowsNotFoundException()
        {
            // Arrange
            const int reportId = 99;

            _reportRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(reportId))
                .ReturnsAsync((Report?)null);

            // Act
            var action = () =>
                _service.GetByIdAsync(reportId);

            // Assert
            var exception =
                await Assert.ThrowsAsync<NotFoundException>(
                    action);

            Assert.Equal(
                "Report with Id: 99 not found.",
                exception.Message);

            _mapperMock.Verify(
                mapper =>
                    mapper.Map<ReportResponseDto>(
                        It.IsAny<Report>()),
                Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_WhenReportExists_ReturnsMappedReport()
        {
            // Arrange
            const int reportId = 12;

            var report = new Report
            {
                Id = reportId,
                Title = "Pothole",
                Description =
                    "Large pothole near the intersection.",
                CategoryId = 3,
                ReporterId = "citizen-id",
                StatusId = 1,
                CreatedAt = DateTime.UtcNow
            };

            var expectedResponse =
                new ReportResponseDto
                {
                    Id = report.Id,
                    Title = report.Title,
                    Description = report.Description,
                    CategoryId = report.CategoryId,
                    ReporterId = report.ReporterId,
                    StatusId = report.StatusId,
                    CreatedAt = report.CreatedAt
                };

            _reportRepositoryMock
                .Setup(repository =>
                    repository.GetByIdAsync(reportId))
                .ReturnsAsync(report);

            _mapperMock
                .Setup(mapper =>
                    mapper.Map<ReportResponseDto>(report))
                .Returns(expectedResponse);

            // Act
            var result =
                await _service.GetByIdAsync(reportId);

            // Assert
            Assert.Same(expectedResponse, result);
            Assert.Equal(reportId, result.Id);
            Assert.Equal("Pothole", result.Title);

            _reportRepositoryMock.Verify(
                repository =>
                    repository.GetByIdAsync(reportId),
                Times.Once);

            _mapperMock.Verify(
                mapper =>
                    mapper.Map<ReportResponseDto>(report),
                Times.Once);
        }

        private static ReportCreateDto CreateValidDto()
        {
            var image = new Mock<IFormFile>();

            image
                .Setup(file => file.FileName)
                .Returns("original.jpg");

            image
                .Setup(file => file.ContentType)
                .Returns("image/jpeg");

            return new ReportCreateDto
            {
                Title = "  Broken street light  ",
                Description =
                    "  The street light is not working.  ",
                CategoryId = 2,
                Latitude = 44.8125,
                Longitude = 20.4612,
                AddressFallback = "  Main Street 12  ",
                Images = new List<IFormFile>
                {
                    image.Object
                }
            };
        }
    }
}
