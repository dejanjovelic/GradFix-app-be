using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Services.Dtos;

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
                dest => dest.PrimaryImage,
                opt => opt.MapFrom(
                    src => src.Images
                        .OrderBy(image => image.Order)
                        .FirstOrDefault()));

            CreateMap<Report, ReportMapItemDto>()
            .ForMember(
                dest => dest.Latitude,
                opt => opt.MapFrom(
                    src => src.Latitude!.Value))
            .ForMember(
                dest => dest.Longitude,
                opt => opt.MapFrom(
                    src => src.Longitude!.Value))
            .ForMember(
                dest => dest.PrimaryImage,
                opt => opt.MapFrom(
                    src => src.Images
                        .OrderBy(image => image.Order)
                        .FirstOrDefault()));
        }
    }
}
