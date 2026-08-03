using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.Mappings
{
    public class ReportStatusProfile : Profile
    {
        public ReportStatusProfile()
        {
            CreateMap<ReportStatus, ReportStatusShortDto>();
        }
    }
}
