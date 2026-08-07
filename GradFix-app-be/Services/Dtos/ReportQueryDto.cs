using System.ComponentModel.DataAnnotations;

namespace GradFix_app_be.Services.Dtos
{
    public class ReportQueryDto
    {
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 50)]
        public int PageSize { get; set; } = 6;

        [Range(1, int.MaxValue)]
        public int? CategoryId { get; set; }

        [Range(1, int.MaxValue)]
        public int? StatusId { get; set; }

        public string? SearchQuery { get; set; }
    }
}
