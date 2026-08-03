namespace GradFix_app_be.Services.Dtos
{
    public class ProfileDto
    {
        public string Id { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }
}
