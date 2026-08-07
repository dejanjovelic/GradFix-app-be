using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Domain.IRepositories;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Exceptions;
using GradFix_app_be.Services.IServices;

namespace GradFix_app_be.Services
{


    public class ReportStatusService : IReportStatusService
    {
        private readonly IReportStatusRepository _reportStatusRepository;
        private readonly IMapper _mapper;

        public ReportStatusService(IReportStatusRepository reportStatusRepository, IMapper mapper)
        {
            _reportStatusRepository = reportStatusRepository;
            _mapper = mapper;
        }

        public async Task<ReportStatusShortDto> GetByNameAsync(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
            {
                throw new BadRequestException("Invalid data status name can not be empty.");
            }
            ReportStatus reportStatus = await _reportStatusRepository.GetByNameAsync(statusName);

            if (reportStatus == null)
            {
                throw new NotFoundException($"Report status with name: {statusName} not found.");
            }

            return _mapper.Map<ReportStatusShortDto>(reportStatus);
        }

        public async Task<IReadOnlyCollection<ReportStatusShortDto>> GetAllAsync()
        {
            var statuses = await _reportStatusRepository.GetAllAsync();

            return _mapper.Map<
                List<ReportStatusShortDto>>(statuses);
        }
    }
}
