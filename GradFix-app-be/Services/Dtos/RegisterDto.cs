using System.ComponentModel.DataAnnotations;

namespace GradFix_app_be.Services.Dtos
{
    public class RegisterDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required, MinLength(8)]
        public string Password { get; set; } = null!;

        public string? Name { get; set; }
        public string? Surname { get; set; }
    }
}
