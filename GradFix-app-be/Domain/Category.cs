using System.Collections.Generic;

namespace GradFix_app_be.Domain
{
    public class Category
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        // Navigation
        public ICollection<Report>? Reports { get; set; }
    }
}
