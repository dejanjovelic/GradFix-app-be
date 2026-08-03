using AutoMapper;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Domain;

namespace GradFix_app_be.Services.Mappings
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<ReportCreateDto, Report>()
                .ForMember(dest => dest.StatusId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()
                );
            CreateMap<Report, ReportResponseDto>();

            CreateMap<Report, ReportListItemDto>()
            .ForMember(
                destination => destination.PrimaryImage,
                options => options.MapFrom(
                    source => source.Images
                        .OrderBy(image => image.Order)
                        .FirstOrDefault()));
        }
    }
}
