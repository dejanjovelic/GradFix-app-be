using GradFix_app_be.Services.Dtos;

namespace GradFix_app_be.Services.Mappings
{
    public class ReportListItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public CategoryShortDto? Category { get; set; }
        public int StatusId { get; set; }
        public ReportStatusShortDto? Status { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AddressFallback { get; set; }
        public DateTime CreatedAt { get; set; }
        public ReportImageResponseDto? PrimaryImage { get; set; }
    }
}
