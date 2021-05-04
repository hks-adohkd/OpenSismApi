namespace OpenSismApi.Models
{
    public class PaginationViewModel
    {
        public int Id { get; set; }

        public int Page { get; set; }

        public int Limit { get; set; }

        public string Query { get; set; }

        public int? TaskTypeId { get; set; }

        public int? PrizeTypeId { get; set; }

        public int? StatusId { get; set; }

        public int? CountryId { get; set; }
    }
}
