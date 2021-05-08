namespace OpenSismApi.Models
{
    public class PagedContent<T>
    {
        public PagedContent()
        {
        }

        public PagedContent(Pagination pagination, T content)
        {
            this.pagination = pagination;
            this.content = content;
        }

        public Pagination pagination { get; set; }
        public T content { get; set; }
    }
}
