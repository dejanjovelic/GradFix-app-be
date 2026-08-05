using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GradFix_app_be.Services.Dtos
{
    public class ReportCreateDto : IValidatableObject
    {
        [StringLength(120, ErrorMessage = "Title cannot exceed 120 characters.")]
        public string? Title { get; set; }

        [Required]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must contain between 10 and 2000 characters.")]
        public string Description { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        [Range(-90.0, 90.0)]
        public double? Latitude { get; set; }

        [Range(-180.0, 180.0)]
        public double? Longitude { get; set; }

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        public string? AddressFallback { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one image is required.")]
        [MaxLength(3, ErrorMessage = "A maximum of three images is allowed.")]
        public List<IFormFile> Images { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(
    ValidationContext validationContext)
        {
            if (!Latitude.HasValue ||
                !Longitude.HasValue)
            {
                yield return new ValidationResult(
                    "Please select the report location on the map or use your current location.",
                    [
                        nameof(Latitude),
                        nameof(Longitude)
                    ]);
            }
        }
    }
}