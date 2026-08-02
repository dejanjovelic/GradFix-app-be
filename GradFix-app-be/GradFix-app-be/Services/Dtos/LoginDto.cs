using System.ComponentModel.DataAnnotations;

namespace GradFix_app_be.Services.Dtos
{
    public class LoginDto
    {
        [Required, EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
