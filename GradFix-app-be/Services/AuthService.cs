using AutoMapper;
using Google.Apis.Auth;
using GradFix_app_be.Domain;
using GradFix_app_be.Services.Dtos;
using GradFix_app_be.Services.Exceptions;
using GradFix_app_be.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Immutable;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GradFix_app_be.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ILogger<AuthService> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;

        private const string DefaultRole = "Citizen";

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ITokenService tokenService,
            IMapper mapper,
            IConfiguration config,
            ILogger<AuthService> logger,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _config = config;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<TokenResultDto> RegisterAsync(RegisterDto dto)
        {
            var user = _mapper.Map<ApplicationUser>(dto);
            user.Email = dto.Email;
            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.UserName = dto.Email;

            var createResult = await _userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                throw new BadRequestException($"Registration failed: {errors}");
            }

            if (!await _roleManager.RoleExistsAsync(DefaultRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(DefaultRole));
            }

            await _userManager.AddToRoleAsync(user, DefaultRole);

            return await _tokenService.CreateTokenAsync(user);
        }

        public async Task<TokenResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                throw new UnauthorizedException("Invalid credentials.");

            var check = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);
            if (!check.Succeeded)
                throw new UnauthorizedException("Invalid credentials.");

            return await _tokenService.CreateTokenAsync(user);
        }

        public async Task<ProfileDto?> GetProfileAsync(ClaimsPrincipal principal)
        {
            var user = await _userManager.GetUserAsync(principal);
            if (user == null)
            {
                var userId = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;
                throw new NotFoundException($"User with Id: {userId} not found.");
            }

            var dto = _mapper.Map<ProfileDto>(user);
            var roles = await _userManager.GetRolesAsync(user);
            dto.Roles = roles.ToList();

            return dto;
        }

        public async Task<TokenResultDto> GoogleSignInAsync(GoogleAuthDto dto)
        {
            var clientId = Environment.GetEnvironmentVariable("Google__ClientId");
            if (string.IsNullOrEmpty(clientId))
                throw new InvalidOperationException("Google:ClientId is not configured.");

            GoogleJsonWebSignature.Payload payload;
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { clientId }
                };
                payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken, settings);
            }
            catch (Exception ex)
            {
                _logger.LogError(
       ex,
       "Google token validation failed. ClientId: {ClientId}, Error: {Message}",
       clientId,
       ex.Message);
                throw new UnauthorizedException("Invalid Google token.");
            }

            if (string.IsNullOrEmpty(payload.Email) || payload.EmailVerified != true)
                throw new UnauthorizedException("Google account email not verified.");

            var user = await _userManager.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    Email = payload.Email,
                    UserName = payload.Email,
                    EmailConfirmed = true,
                    Name = payload.GivenName,
                    Surname = payload.FamilyName
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    throw new BadRequestException($"Failed to create user from Google account: {errors}");
                }
            }

            if (!await _roleManager.RoleExistsAsync(DefaultRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(DefaultRole));
            }

            await _userManager.AddToRoleAsync(user, DefaultRole);

            return await _tokenService.CreateTokenAsync(user);
        }
    }
}
