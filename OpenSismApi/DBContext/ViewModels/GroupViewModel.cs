namespace DBContext.ViewModels
{
    public partial class GroupViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public int ItemOrder { get; set; }
        public string Color { get; set; }
        public int Points { get; set; }
        public string ImageUrl { get; set; }

    }
}
