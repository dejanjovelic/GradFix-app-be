using GradFix_app_be.Domain;

namespace GradFix_app_be.Services.Dtos
{
    public class CategoryShortDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

    }
}
