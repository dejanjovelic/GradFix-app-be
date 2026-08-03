using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.Mappings
{
    public class ReportStatusHistoryProfile : Profile
    {
        public ReportStatusHistoryProfile()
        {
            CreateMap<ReportStatusHistory, ReportStatusHistoryResponseDto>();
        }
    }
}
