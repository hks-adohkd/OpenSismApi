namespace DBContext.ViewModels
{
    public partial class ApplicationUserViewModel
    {
        public bool PhoneNumberConfirmed { get; set; }

        public string PhoneNumber { get; set; }

        public bool EmailConfirmed { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }
    }
}
