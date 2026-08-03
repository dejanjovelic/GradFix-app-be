using GradFix_app_be.Domain;

namespace GradFix_app_be.Services.Dtos
{
    public class ReportStatusHistoryResponseDto
    {
        public int Id { get; set; }

        public int ReportId { get; set; }

        public int? OldStatusId { get; set; }
        public ReportStatusShortDto? OldStatus { get; set; }

        public int NewStatusId { get; set; }
        public ReportStatusShortDto NewStatus { get; set; } = null!;

        public string? ChangedByUserId { get; set; }
        public UserShortDto? ChangedByUser { get; set; }

        public string? Comment { get; set; }

        public DateTime ChangedAt { get; set; }

    }
}
