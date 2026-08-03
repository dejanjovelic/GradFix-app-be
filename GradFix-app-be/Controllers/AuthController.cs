using System.Threading.Tasks;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GradFix_app_be.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _auth;

        public AuthController(IAuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var token = await _auth.RegisterAsync(dto);
            return Ok(token);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var token = await _auth.LoginAsync(dto);
            return Ok(token);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var profile = await _auth.GetProfileAsync(User);
            return Ok(profile);
        }

        [HttpPost("google")]
        public async Task<IActionResult> Google([FromBody] GoogleAuthDto dto)
        {
            var token = await _auth.GoogleSignInAsync(dto);
            return Ok(token);
        }
    }
}
