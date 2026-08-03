namespace GradFix_app_be.Services.Dtos
{
    public class PaginatedListDto<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public PaginatedListDto(
            List<T> items,
            int page,
            int pageSize,
            int totalRowCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalCount = totalRowCount;

            TotalPages = (int)Math.Ceiling(
                totalRowCount / (decimal)pageSize);
        }
    }
}
