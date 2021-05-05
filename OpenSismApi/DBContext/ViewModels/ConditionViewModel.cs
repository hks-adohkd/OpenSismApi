namespace DBContext.ViewModels
{
    public partial class ConditionViewModel : BaseEntityViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }
    }
}
