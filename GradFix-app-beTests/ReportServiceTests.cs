using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Domain.IRepositories;
using GradFix_app_be.Infrastructure.Repositories;
using GradFix_app_be.Services;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Exceptions;
using GradFix_app_be.Services.IServices;
using Microsoft.AspNetCore.Http;
using Moq;
using Shouldly;
using System.Timers;

namespace GradFix_app_beTests
{
    public class ReportServiceTests
    {
        private readonly Mock<IReportRepository> _reportRepositoryMock;
        private readonly Mock<IReportStatusRepository> _reportStatusRepositoryMock;
        private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
        private readonly Mock<IImageStorageService> _imageStorageServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IReportStatusHistoryRepository> _reportStatusHistoryRepository;
        private readonly Mock<IUnitOfWork> _unitOfWork;


        private readonly ReportService _service;

        public ReportServiceTests()
        {
            _reportRepositoryMock = new Mock<IReportRepository>();
            _reportStatusRepositoryMock = new Mock<IReportStatusRepository>();
            _categoryRepositoryMock = new Mock<ICategoryRepository>();
            _reportStatusHistoryRepository = new Mock<IReportStatusHistoryRepository>();
            _imageStorageServiceMock = new Mock<IImageStorageService>();
            _mapperMock = new Mock<IMapper>();

            _unitOfWork = new Mock<IUnitOfWork>();

            _service = new ReportService(
                _reportRepositoryMock.Object,
                _reportStatusRepositoryMock.Object,
                _categoryRepositoryMock.Object,
                _reportStatusHistoryRepository.Object,
                _imageStorageServiceMock.Object,
                _mapperMock.Object,
                _unitOfWork.Object);
        }

        [Fact]
        public async Task CreateReportAsync_WhenReporterIdIsMissing_ThrowsUnauthorizedException()
        {
            // Arrange
            var dto = CreateValidDto();

            // Act
            var action = () =>
                _service.CreateAsync(dto, null);

            // Assert
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(action);

            Assert.Equal("Authenticated user identifier is missing.", exception.Message);

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
                _service.CreateAsync(dto, "citizen-id");

            // Assert
            var exception = await Assert.ThrowsAsync<BadRequestException>(action);

            Assert.Equal("The selected category does not exist.", exception.Message);

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
                _service.CreateAsync(
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
                await _service.CreateAsync(
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

        [Fact]
        public async Task GetAllAsync_WithValidQuery_ReturnsPaginatedReports()
        {
            // Arrange
            var query = new ReportQueryDto
            {
                Page = 1,
                PageSize = 2,
                CategoryId = 1,
                StatusId = 1
            };

            var reports = new List<Report>
                        {
                            new()
                            {
                                Id = 1,
                                Title = "Broken bench",
                                Description = "Bench is damaged.",
                                CategoryId = 1,
                                StatusId = 1,
                                CreatedAt = DateTime.UtcNow
                            },
                            new()
                            {
                                Id = 2,
                                Title = "Broken bin",
                                Description = "Public bin is damaged.",
                                CategoryId = 1,
                                StatusId = 1,
                                CreatedAt = DateTime.UtcNow
                            }
                        };

            var paginatedReports = new PaginatedList<Report>(
                reports,
                page: 1,
                pageSize: 2,
                totalRowCount: 5);

            var mappedReports = new List<ReportListItemDto>
                        {
                            new()
                            {
                                Id = 1,
                                Title = "Broken bench",
                                Description = "Bench is damaged.",
                                CategoryId = 1,
                                StatusId = 1
                            },
                            new()
                            {
                                Id = 2,
                                Title = "Broken bin",
                                Description = "Public bin is damaged.",
                                CategoryId = 1,
                                StatusId = 1
                            }
                        };

            _reportRepositoryMock
                .Setup(repository => repository.GetAllAsync(
                    query.Page,
                    query.PageSize,
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<string?>()))
                .ReturnsAsync(paginatedReports);

            _mapperMock
                .Setup(mapper =>
                    mapper.Map<List<ReportListItemDto>>(reports))
                .Returns(mappedReports);

            // Act
            var result = await _service.GetAllAsync(query);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Items.Count);
            Assert.Equal(1, result.Page);
            Assert.Equal(2, result.PageSize);
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(3, result.TotalPages);

            Assert.Equal(1, result.Items[0].Id);
            Assert.Equal("Broken bench", result.Items[0].Title);

            _reportRepositoryMock.Verify(repository =>
                repository.GetAllAsync(
                    1,
                    2,
                    1,
                    1,
                    null
                    ),
                Times.Once);

            _mapperMock.Verify(mapper =>
                mapper.Map<List<ReportListItemDto>>(reports),
                Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoReportsExist_ReturnsEmptyPaginatedList()
        {
            // Arrange
            var query = new ReportQueryDto
            {
                Page = 1,
                PageSize = 6
            };

            var reports = new List<Report>();

            var paginatedReports = new PaginatedList<Report>(
                reports,
                page: 1,
                pageSize: 6,
                totalRowCount: 0);

            _reportRepositoryMock
                .Setup(repository => repository.GetAllAsync(
                    query.Page,
                    query.PageSize,
                    It.IsAny<int?>(),
                     It.IsAny<int?>(),
                      It.IsAny<string?>()
                     ))
                .ReturnsAsync(paginatedReports);

            _mapperMock
                .Setup(mapper =>
                    mapper.Map<List<ReportListItemDto>>(reports))
                .Returns([]);

            // Act
            var result = await _service.GetAllAsync(query);

            // Assert
            Assert.Empty(result.Items);
            Assert.Equal(1, result.Page);
            Assert.Equal(6, result.PageSize);
            Assert.Equal(0, result.TotalCount);
            Assert.Equal(0, result.TotalPages);

            _reportRepositoryMock.Verify(repository =>
                repository.GetAllAsync(
                    1,
                    6,
                    null,
                    null,
                    null),
                Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithCategoryFilter_PassesCategoryToRepository()
        {
            // Arrange
            var query = new ReportQueryDto
            {
                Page = 2,
                PageSize = 6,
                CategoryId = 3,
                StatusId = null
            };

            var paginatedReports = new PaginatedList<Report>(
                [],
                page: 2,
                pageSize: 6,
                totalRowCount: 0);

            _reportRepositoryMock
                .Setup(repository => repository.GetAllAsync(
                    2,
                    6,
                    3,
                    null,
                    null))
                .ReturnsAsync(paginatedReports);

            _mapperMock
                .Setup(mapper =>
                    mapper.Map<List<ReportListItemDto>>(
                        paginatedReports.Items))
                .Returns([]);

            // Act
            await _service.GetAllAsync(query);

            // Assert
            _reportRepositoryMock.Verify(repository =>
                repository.GetAllAsync(
                    2,
                    6,
                    3,
                    null,
                    null
                    ),
                Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_WithStatusFilter_PassesStatusToRepository()
        {
            // Arrange
            var query = new ReportQueryDto
            {
                Page = 1,
                PageSize = 10,
                CategoryId = null,
                StatusId = 2
            };

            var paginatedReports = new PaginatedList<Report>(
                [],
                page: 1,
                pageSize: 10,
                totalRowCount: 0);

            _reportRepositoryMock
                .Setup(repository => repository.GetAllAsync(
                    1,
                    10,
                    null,
                    2,
                    null
                    ))
                .ReturnsAsync(paginatedReports);

            _mapperMock
                .Setup(mapper =>
                    mapper.Map<List<ReportListItemDto>>(
                        paginatedReports.Items))
                .Returns([]);

            // Act
            await _service.GetAllAsync(query);

            // Assert
            _reportRepositoryMock.Verify(repository =>
                repository.GetAllAsync(
                    1,
                    10,
                    null,
                    2,
                    null),
                Times.Once);
        }

        [Theory]
        [InlineData(0, 6, 0)]
        [InlineData(1, 6, 1)]
        [InlineData(6, 6, 1)]
        [InlineData(7, 6, 2)]
        [InlineData(12, 6, 2)]
        [InlineData(13, 6, 3)]
        public void PaginatedListDto_CalculatesTotalPagesCorrectly(
        int totalCount,
        int pageSize,
        int expectedTotalPages)
        {
            // Act
            var result =
                new PaginatedListDto<ReportListItemDto>(
                    [],
                    page: 1,
                    pageSize,
                    totalCount);

            // Assert
            Assert.Equal(expectedTotalPages, result.TotalPages);
            Assert.Equal(totalCount, result.TotalCount);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetMineAsync_WhenReporterIdMissing_ThrowsUnauthorized(string? reporterId)
        {
            // Arrange
            var query = new ReportQueryDto();

            // Act
            var action = async () =>
                await _service.GetMineAsync(
                    query,
                    reporterId);

            // Assert
            await action.ShouldThrowAsync<UnauthorizedException>();
        }

        [Fact]
        public async Task GetMineAsync_PassesReporterIdToRepository()
        {
            // Arrange
            const string reporterId = "citizen-123";

            var query = new ReportQueryDto
            {
                Page = 1,
                PageSize = 6
            };

            var reports = new List<Report>
    {
        new Report
        {
            Id = 1,
            ReporterId = reporterId,
            Title = "First report"
        },
        new Report
        {
            Id = 2,
            ReporterId = reporterId,
            Title = "Second report"
        }
    };

            var paginatedReports = new PaginatedList<Report>(
                reports,
                1,
                6,
                2);

            _reportRepositoryMock
                .Setup(repository => repository.GetMineAsync(
                    reporterId,
                    query.Page,
                    query.PageSize,
                    query.CategoryId,
                    query.StatusId,
                    query.SearchQuery))
                .ReturnsAsync(paginatedReports);

            _mapperMock
    .Setup(mapper =>
        mapper.Map<List<ReportListItemDto>>(reports))
    .Returns(new List<ReportListItemDto>
    {
                new()
                {
                    Id = 7,
                    Title = "Broken street light"
                },
                new()
                {
                    Id = 8,
                    Title = "Damaged bench"
                }
    });

            // Act
            var result = await _service.GetMineAsync(
                query,
                reporterId);

            // Assert
            _reportRepositoryMock.Verify(
                repository => repository.GetMineAsync(
                    reporterId,
                    query.Page,
                    query.PageSize,
                    query.CategoryId,
                    query.StatusId,
                    query.SearchQuery),
                Times.Once);

            result.Items.Count.ShouldBe(2);
        }

        [Fact]
        public async Task GetMineAsync_MapsPaginatedResultCorrectly()
        {
            // Arrange
            const string reporterId = "citizen-123";

            var query = new ReportQueryDto
            {
                Page = 2,
                PageSize = 6
            };

            var reports = new List<Report>
            {
            new Report
            {
                Id = 7,
                ReporterId = reporterId,
                Title = "Broken street light",
                Description = "Street light is not working.",
                CategoryId = 2,
                StatusId = 1,
                CreatedAt = DateTime.UtcNow
            },
            new Report
            {
                Id = 8,
                ReporterId = reporterId,
                Title = "Damaged bench",
                Description = "Bench is damaged.",
                CategoryId = 1,
                StatusId = 1,
                CreatedAt = DateTime.UtcNow
            }
        };

            var paginatedReports = new PaginatedList<Report>(
                reports,
                page: 2,
                pageSize: 6,
                totalRowCount: 14);

            _reportRepositoryMock
                .Setup(repository => repository.GetMineAsync(
                    reporterId,
                    query.Page,
                    query.PageSize,
                    query.CategoryId,
                    query.StatusId,
                    query.SearchQuery))
                .ReturnsAsync(paginatedReports);

            _mapperMock
                .Setup(mapper =>
                    mapper.Map<List<ReportListItemDto>>(reports))
                .Returns(new List<ReportListItemDto>
                {
                    new()
                    {
                        Id = 7,
                        Title = "Broken street light"
                    },
                    new()
                    {
                        Id = 8,
                        Title = "Damaged bench"
                    }
                });

            // Act
            var result = await _service.GetMineAsync(
                    query,
                    reporterId);

            // Assert
            result.Page.ShouldBe(2);
            result.PageSize.ShouldBe(6);
            result.TotalCount.ShouldBe(14);
            result.TotalPages.ShouldBe(3);

            result.Items.Count.ShouldBe(2);

            result.Items[0].Id.ShouldBe(7);
            result.Items[0].Title.ShouldBe("Broken street light");

            result.Items[1].Id.ShouldBe(8);
            result.Items[1].Title.ShouldBe("Damaged bench");
        }
    }
}
