using System.Collections.Generic;

namespace DBContext.ViewModels
{
    public partial class QuizPageViewModel  
    {
       // public virtual List<AppTaskViewModel> FinishedTasks { get; set; }
       // public virtual List<AppTaskViewModel> Tasks { get; set; }
      //  public virtual List<AppTaskViewModel> PendingTasks { get; set; }
        public virtual List<QuizViewModel> Quizes { get; set; }

        public virtual QuizIndexViewModel Indexes { get; set; } 

        public int TotalQuestionsCount { get; set; }


        
    }
}
