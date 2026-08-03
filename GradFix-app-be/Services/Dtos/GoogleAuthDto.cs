using System.ComponentModel.DataAnnotations;

namespace GradFix_app_be.Services.Dtos
{
    public class GoogleAuthDto
    {
        [Required]
        public string IdToken { get; set; } = null!;
    }
}
