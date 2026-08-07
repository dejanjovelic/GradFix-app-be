using System;

namespace GradFix_app_be.Domain
{
    public class ReportImage
    {
        public int Id { get; set; }

        public int ReportId { get; set; }

        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? ContentType { get; set; }
        public int Size { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int Order { get; set; }
    }
}
