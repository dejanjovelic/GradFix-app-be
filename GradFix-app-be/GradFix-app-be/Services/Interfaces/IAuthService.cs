using System.Security.Claims;
using System.Threading.Tasks;
using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResultDto> RegisterAsync(RegisterDto dto);
        Task<TokenResultDto> LoginAsync(LoginDto dto);
        Task<ProfileDto?> GetProfileAsync(ClaimsPrincipal principal);
        Task<TokenResultDto> GoogleSignInAsync(GoogleAuthDto dto);
    }
}
