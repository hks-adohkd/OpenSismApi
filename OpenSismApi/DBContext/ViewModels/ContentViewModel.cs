/// <summary>
/// for slider and banner
/// </summary>

namespace DBContext.ViewModels
{
    public partial class ContentViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Script { get; set; }
        public string ImageUrl { get; set; }
        public int ItemOrder { get; set; }
    }
}
