using System;
using System.Collections.Generic;

namespace GradFix_app_be.Domain
{
    public class ReportStatusHistory
    {
        public int Id { get; set; }

        public int ReportId { get; set; }
        public Report? Report { get; set; }

        public int? OldStatusId { get; set; }
        public ReportStatus? OldStatus { get; set; }

        public int NewStatusId { get; set; }
        public ReportStatus? NewStatus { get; set; }

        // Identity user who changed status (nullable)
        public string? ChangedByUserId { get; set; }
        public ApplicationUser? ChangedByUser { get; set; }

        public string? Comment { get; set; }
        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;
    }
}
