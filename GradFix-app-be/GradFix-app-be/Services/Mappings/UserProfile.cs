using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname));

            CreateMap<ApplicationUser, ProfileDto>()
                .ForMember(d => d.Name, opt => opt.MapFrom(s => s.Name))
                .ForMember(d => d.Surname, opt => opt.MapFrom(s => s.Surname));
        }
    }
}
