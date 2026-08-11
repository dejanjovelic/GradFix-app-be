
using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Domain.IRepositories;
using GradFix_app_be.Infrastructure.Repositories;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Exceptions;
using GradFix_app_be.Services.IServices;

namespace GradFix_app_be.Services
{
    public class ReportService : IReportService
    {
        private const string InitialStatusName = "New";

        private readonly IReportRepository _reportRepository;
        private readonly IReportStatusRepository _reportStatusRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IReportStatusHistoryRepository _reportStatusHistoryRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public ReportService(
            IReportRepository reportRepository,
            IReportStatusRepository reportStatusRepository,
            ICategoryRepository categoryRepository,
            IReportStatusHistoryRepository reportStatusHistoryRepository,
            IImageStorageService imageStorageService,
            IMapper mapper,
            IUnitOfWork unitOfWork
            )
        {
            _reportRepository = reportRepository;
            _reportStatusRepository = reportStatusRepository;
            _categoryRepository = categoryRepository;
            _reportStatusHistoryRepository = reportStatusHistoryRepository;
            _imageStorageService = imageStorageService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<ReportResponseDto> CreateAsync(ReportCreateDto dto, string? reporterId)
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
            await _unitOfWork.SaveAsync();

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
                    query.StatusId,
                    query.SearchQuery);

            var reportDtos = _mapper.Map<List<ReportListItemDto>>(paginatedReports.Items);

            return new PaginatedListDto<ReportListItemDto>(
                reportDtos,
                paginatedReports.Page,
                paginatedReports.PageSize,
                paginatedReports.TotalCount);
        }

        public async Task<IReadOnlyCollection<ReportListItemDto>> GetMapItemsAsync(int? categoryId, int? statusId)
        {
            var reports = await _reportRepository.GetMapItemsAsync(categoryId, statusId);

            return _mapper.Map<List<ReportListItemDto>>(reports);
        }

        public async Task<ReportResponseDto> UpdateStatusAsync(int reportId, ReportStatusUpdateDto dto, string? changedByUserId)
        {
            if (string.IsNullOrWhiteSpace(changedByUserId))
            {
                throw new UnauthorizedException("Authenticated user identifier is missing.");
            }

            var report = await _reportRepository.GetForStatusUpdateAsync(reportId);

            if (report == null)
            {
                throw new NotFoundException($"Report with Id: {reportId} not found.");
            }

            var newStatus = await _reportStatusRepository.GetByIdAsync(dto.StatusId);

            if (newStatus == null)
            {
                throw new BadRequestException("The selected report status does not exist.");
            }

            if (report.StatusId == newStatus.Id)
            {
                throw new BadRequestException($"The report already has status '{newStatus.Name}'.");
            }

            var oldStatusId = report.StatusId;
            var changedAt = DateTime.UtcNow;

            var statusHistory =
                new ReportStatusHistory
                {
                    ReportId = report.Id,
                    OldStatusId = oldStatusId,
                    NewStatusId = newStatus.Id,
                    ChangedByUserId = changedByUserId,
                    Comment = dto.Comment?.Trim(),
                    ChangedAt = changedAt
                };

            report.StatusId = newStatus.Id;
            report.UpdatedAt = changedAt;

            await _reportStatusHistoryRepository.AddAsync(statusHistory);

            await _unitOfWork.SaveAsync();

            return await GetByIdAsync(report.Id);
        }

        public async Task<PaginatedListDto<ReportListItemDto>> GetMineAsync(ReportQueryDto query, string? reporterId) 
        {
            if (string.IsNullOrWhiteSpace(reporterId))
            {
                throw new UnauthorizedException(
                    "Authenticated user identifier is missing.");
            }

            var paginatedReports =
                await _reportRepository.GetMineAsync(
                    reporterId,
                    query.Page,
                    query.PageSize,
                    query.CategoryId,
                    query.StatusId,
                    query.SearchQuery);

            var reportDtos =
                _mapper.Map<List<ReportListItemDto>>(
                    paginatedReports.Items);

            return new PaginatedListDto<ReportListItemDto>(
                reportDtos,
                paginatedReports.Page,
                paginatedReports.PageSize,
                paginatedReports.TotalCount);
        }
    }
}