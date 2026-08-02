namespace GradFix_app_be.Domain
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public PaginatedList(List<T> items, int page, int pageSize, int totalRowCount)
        {
            Items = items;
            Page = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalRowCount / (decimal)pageSize);
        }
    }
}
