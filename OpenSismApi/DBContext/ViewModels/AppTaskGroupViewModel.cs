namespace DBContext.ViewModels
{
    public partial class AppTaskGroupViewModel : BaseEntityViewModel
    {
        public int AppTaskId { get; set; }
        public int GroupId { get; set; }

        public virtual GroupViewModel Group { get; set; }
    }
}
