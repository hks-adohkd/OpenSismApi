namespace DBContext.ViewModels
{
    public partial class MessageViewModel : BaseEntityViewModel
    {
        public string Title { get; set; }
        public string Script { get; set; }
        public int? GroupId { get; set; }
        public bool IsForAll { get; set; }

        public virtual GroupViewModel Group { get; set; }
    }
}
