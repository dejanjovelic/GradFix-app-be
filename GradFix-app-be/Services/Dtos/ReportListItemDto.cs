namespace GradFix_app_be.Services.Dtos
{
    public class ReportListItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string Description { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int StatusId { get; set; }
        public string StatusName { get; set; } = null!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? AddressFallback { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? PrimaryImagePath { get; set; }
    }
}
