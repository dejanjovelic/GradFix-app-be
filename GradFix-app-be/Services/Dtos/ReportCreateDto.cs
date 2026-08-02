using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GradFix_app_be.Services.Dtos
{
    public class ReportCreateDto
    {
        public string? Title { get; set; }

        [Required]
        public string Description { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? AddressFallback { get; set; }

        // Image metadata. Enforcement of max 3 images is done in service layer.
        public List<ReportImageCreateDto>? Images { get; set; }
    }
}
