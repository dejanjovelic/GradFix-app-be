namespace GradFix_app_be.Services.Dtos
{
    public class StoredImageDto
    {
        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string ContentType { get; set; } = null!;
        public int Size { get; set; }
        public int Order { get; set; }
    }
}
