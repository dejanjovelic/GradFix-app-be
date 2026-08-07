
using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Domain.IRepositories;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Exceptions;
using GradFix_app_be.Services.IServices;
using GradFix_app_be.Services.Mappings;

namespace GradFix_app_be.Services
{
    public class ReportService : IReportService
    {
        private const string InitialStatusName = "New";

        private readonly IReportRepository _reportRepository;
        private readonly IReportStatusRepository _reportStatusRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly IMapper _mapper;

        public ReportService(
            IReportRepository reportRepository,
            IReportStatusRepository reportStatusRepository,
            ICategoryRepository categoryRepository,
            IImageStorageService imageStorageService,
            IMapper mapper
            )
        {
            _reportRepository = reportRepository;
            _reportStatusRepository = reportStatusRepository;
            _categoryRepository = categoryRepository;
            _imageStorageService = imageStorageService;
            _mapper = mapper;
        }

        public async Task<ReportResponseDto> CreateReportAsync(ReportCreateDto dto, string? reporterId)
        {
            if (string.IsNullOrWhiteSpace(reporterId))
            {
                throw new UnauthorizedException("Authenticated user identifier is missing.");
            }

            var categoryExists = await _categoryRepository.ExistsAsync(dto.CategoryId);

            if (!categoryExists)
            {
                throw new BadRequestException(
                    "The selected category does not exist.");
            }

            var initialStatus = await _reportStatusRepository.GetByNameAsync(InitialStatusName);

            if (initialStatus == null)
            {
                throw new InvalidOperationException(
                    $"The initial report status '{InitialStatusName}' is not configured.");
            }

            var storedImages = await _imageStorageService.SaveReportImagesAsync(dto.Images);

            var report = new Report
            {
                Title = dto.Title?.Trim(),
                Description = dto.Description.Trim(),
                CategoryId = dto.CategoryId,
                ReporterId = reporterId,
                StatusId = initialStatus.Id,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                AddressFallback = dto.AddressFallback?.Trim(),
                CreatedAt = DateTime.UtcNow,

                Images = storedImages
                    .Select(image => new ReportImage
                    {
                        FileName = image.FileName,
                        FilePath = image.FilePath,
                        ContentType = image.ContentType,
                        Size = image.Size,
                        Order = image.Order,
                        CreatedAt = DateTime.UtcNow
                    }).ToList()
            };

            await _reportRepository.AddAsync(report);

            return _mapper.Map<ReportResponseDto>(report);
        }
        public async Task<ReportResponseDto> GetByIdAsync(int id)
        {
            Report? report = await _reportRepository.GetByIdAsync(id);
            if (report == null)
            {
                throw new NotFoundException($"Report with Id: {id} not found.");
            }
            return _mapper.Map<ReportResponseDto>(report);
        }

        public async Task<PaginatedListDto<ReportListItemDto>> GetAllAsync(ReportQueryDto query)
        {
            var paginatedReports =
                await _reportRepository.GetAllAsync(
                    query.Page,
                    query.PageSize,
                    query.CategoryId,
                    query.StatusId);

            var reportDtos = _mapper.Map<List<ReportListItemDto>>(paginatedReports.Items);

            return new PaginatedListDto<ReportListItemDto>(
                reportDtos,
                paginatedReports.Page,
                paginatedReports.PageSize,
                paginatedReports.TotalCount);
        }

        public async Task<IReadOnlyCollection<ReportMapItemDto>> GetMapItemsAsync( int? categoryId, int? statusId)
        {
            var reports = await _reportRepository.GetMapItemsAsync( categoryId, statusId);

            return _mapper.Map<List<ReportMapItemDto>>(reports);
        }
    }
}