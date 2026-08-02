namespace GradFix_app_be.Services.Dtos
{
    public class ReportImageCreateDto
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!; // could be a temporary path or storage key
        public string ContentType { get; set; } = null!;
        public int Size { get; set; }
    }
}
