
namespace DBContext.ViewModels
{
    public partial class CustomerPredictionViewModel : BaseEntityViewModel
    {
        public int CustomerId { get; set; }
        public int SportMatchId { get; set; }
        public int FirstTeamScore { get; set; }
        public int SecondTeamScore { get; set; }

        public virtual SportMatchViewModel SportMatch { get; set; }
    }
}
