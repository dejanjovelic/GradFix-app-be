using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace GradFix_app_be.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public ICollection<Report>? Reports { get; set; }
    }
}
