namespace DBContext.ViewModels
{
    public partial class QuizOptionViewModel : BaseEntityViewModel
    {
        public string Script { get; set; }
        public int QuestionId { get; set; }
        public int ItemOrder { get; set; }
        public bool IsRightOption { get; set; }
    }
}
