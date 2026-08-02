using System.Threading.Tasks;
using GradFix_app_be.Domain;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.Interfaces
{
    public interface ITokenService
    {
        Task<TokenResultDto> CreateTokenAsync(ApplicationUser user);
    }
}
