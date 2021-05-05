
/// <summary>
/// it is for about US social media information
/// </summary>

namespace DBContext.ViewModels
{
    public partial class ContactViewModel : BaseEntityViewModel
    {
        public string Icon { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
    }
}
