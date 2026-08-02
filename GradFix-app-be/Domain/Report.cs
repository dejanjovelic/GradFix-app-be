using System;
using System.Collections.Generic;

namespace GradFix_app_be.Domain
{
    public class Report
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }

        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        // Identity user id (string) - reporter
        public string? ReporterId { get; set; }
        public ApplicationUser? Reporter { get; set; }

        public int StatusId { get; set; }
        public ReportStatus? Status { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public string? AddressFallback { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ReportImage>? Images { get; set; }
        public ICollection<ReportStatusHistory>? StatusHistory { get; set; }
    }
}
