namespace DBContext.ViewModels
{
    public partial class MessageGroupViewModel : BaseEntityViewModel
    {
        public int MessageId { get; set; }
        public int GroupId { get; set; }

        public virtual GroupViewModel Group { get; set; }
    }
}
