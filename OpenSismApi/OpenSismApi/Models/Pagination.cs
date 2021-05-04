namespace OpenSismApi.Models
{
    public class Pagination
    {
        public Pagination()
        {
        }

        public Pagination(int totalCount, int limit, int totalPages, int currentPage)
        {
            TotalCount = totalCount;
            Limit = limit;
            TotalPages = totalPages;
            CurrentPage = currentPage;
        }

        public int TotalCount { get; set; }
        public int Limit { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}
