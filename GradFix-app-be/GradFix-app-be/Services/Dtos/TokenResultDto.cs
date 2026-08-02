using System;

namespace GradFix_app_be.Services.Dtos
{
    public class TokenResultDto
    {
        public string Token { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public ProfileDto Profile { get; set; } = null!;
    }
}
