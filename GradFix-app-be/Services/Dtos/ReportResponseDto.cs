using GradFix_app_be.Domain;

namespace GradFix_app_be.Services.Dtos
{
    public class ReportResponseDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public CategoryShortDto? Category { get; set; }
        public string ReporterId { get; set; } = null!;
        public UserShortDto? Reporter { get; set; }
        public int StatusId { get; set; }
        public ReportStatusShortDto? Status { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AddressFallback { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IReadOnlyCollection<ReportImageResponseDto> Images
        { get; set; } = Array.Empty<ReportImageResponseDto>();

        public IReadOnlyCollection<ReportStatusHistoryResponseDto> StatusHistory
        { get; set; } = Array.Empty<ReportStatusHistoryResponseDto>();
    }
}
