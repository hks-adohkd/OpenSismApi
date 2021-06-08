namespace DBContext.ViewModels
{
    public partial class ContactUsViewModel : BaseEntityViewModel
    {
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        public string FeaturedSubject { get; set; }
        public bool IsFeatured { get; set; }
        public string Reply { get; set; }
        public string FeaturedReply { get; set; }
        public bool IsViewed { get; set; }

        public bool IsReaded { get; set; }
        public int? CustomerId { get; set; }
    }
}
