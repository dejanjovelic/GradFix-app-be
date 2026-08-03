using AutoMapper;
using GradFix_app_be.Domain;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, ApplicationUser>();

            CreateMap<ApplicationUser, ProfileDto>()
                .ForMember(dest=>dest.Roles, opt=>opt.Ignore());
        }
    }
}
