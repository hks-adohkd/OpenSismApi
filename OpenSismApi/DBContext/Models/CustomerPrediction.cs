using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("CustomerPrediction")]
    public partial class CustomerPrediction : BaseEntity
    {
        [Required]
        [Display(Name = "Customer *")]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Sport Match *")]
        public int SportMatchId { get; set; }

        [Required]
        [Display(Name = "First Team Score *")]
        public int FirstTeamScore { get; set; }

        [Required]
        [Display(Name = "Second Team Score *")]
        public int SecondTeamScore { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual SportMatch SportMatch { get; set; }
    }
}
