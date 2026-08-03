using AutoMapper;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Domain;

namespace GradFix_app_be.Services.Mappings
{
    public class ReportImageProfile : Profile
    {
        public ReportImageProfile()
        {
            CreateMap<ReportImage, ReportImageResponseDto>();
        }
    }
}
