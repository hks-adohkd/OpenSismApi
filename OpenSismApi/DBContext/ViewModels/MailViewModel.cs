namespace DBContext.ViewModels
{
    public partial class MailViewModel : BaseEntityViewModel
    {
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string SenderEmail { get; set; }
        public string RecieverEmail { get; set; }
        public string Subject { get; set; }
        public string Message { get; set; }
        
        public int? CustomerId { get; set; }
    }
}
