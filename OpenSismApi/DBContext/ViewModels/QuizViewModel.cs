using System.Collections.Generic;

namespace DBContext.ViewModels
{  
    public partial class QuizViewModel : BaseEntityViewModel
    {
        public string Script { get; set; }
        public string Description { get; set; }
        public bool Type { get; set; }
        public int ItemOrder { get; set; }
        public int AppTaskId { get; set; }
        public bool IsLast { get; set; }
        public bool IsAllRight { get; set; }
        public int TotalQuestionsCount { get; set; }

        //public virtual AppTaskViewModel AppTask { get; set; }
        public virtual List<QuizOptionViewModel> QuizOptions    { get; set; }
        //public virtual List<CustomerAnswerViewModel> CustomerAnswers { get; set; }
    }
}
