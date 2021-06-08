using System.Collections.Generic;

namespace DBContext.ViewModels
{
    public partial class HomePageViewModel
    {
       // public virtual List<AppTaskViewModel> FinishedTasks { get; set; }
       // public virtual List<AppTaskViewModel> Tasks { get; set; }
      //  public virtual List<AppTaskViewModel> PendingTasks { get; set; }
        public virtual List<ContentViewModel> Slides { get; set; }
        public virtual List<ContentViewModel> Banner { get; set; }

        public bool LuckyWheelValid { get; set; }
        public bool DailyBonusValid { get; set; }
        public int NewMessages { get; set; }

        public int NewNotification { get; set; }    
    }
}
