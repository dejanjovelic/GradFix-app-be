using System.ComponentModel.DataAnnotations;

namespace GradFix_app_be.Services.Dtos
{
    public class ReportStatusUpdateDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Status is required.")]
        public int StatusId { get; set; }

        [StringLength(500, ErrorMessage = "Comment cannot exceed 500 characters.")]
        public string? Comment { get; set; }
    }
}
