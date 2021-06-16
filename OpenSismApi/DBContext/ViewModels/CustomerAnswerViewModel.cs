namespace DBContext.ViewModels
{
    public partial class CustomerAnswerViewModel : BaseEntityViewModel
    {
        public int CustomerId { get; set; }
        public int? QuestionOptionId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public bool IsRightAnswer { get; set; }

        public virtual QuestionOptionViewModel QuestionOption { get; set; }
    }
}
