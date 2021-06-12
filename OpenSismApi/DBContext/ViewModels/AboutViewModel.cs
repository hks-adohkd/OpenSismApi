using System.Collections.Generic;

namespace DBContext.ViewModels
{
    public partial class AboutViewModel
    {
       // public virtual List<AppTaskViewModel> FinishedTasks { get; set; }
       // public virtual List<AppTaskViewModel> Tasks { get; set; }
      //  public virtual List<AppTaskViewModel> PendingTasks { get; set; }
        public virtual List<ContactViewModel> Videos { get; set; }
        public virtual List<ContactViewModel> Contacts { get; set; }

        public virtual ContactViewModel Location { get; set; }
        public virtual ContentViewModel AboutUs { get; set; }


    }
}
