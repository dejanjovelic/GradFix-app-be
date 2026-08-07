namespace GradFix_app_be.Services.Dtos
{
    public class ReportMapItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? AddressFallback { get; set; }
        public DateTime CreatedAt { get; set; }
        public CategoryShortDto? Category { get; set; }
        public ReportStatusShortDto? Status { get; set; }
        public ReportImageResponseDto? PrimaryImage { get; set; }
    }
}
