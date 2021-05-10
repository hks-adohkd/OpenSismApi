using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("CustomerAnswer")]
    public partial class CustomerAnswer : BaseEntity
    {
        [Required]
        [Display(Name = "Customer *")]
        public int CustomerId { get; set; }

        [Display(Name = "Option")]
        public int? QuestionOptionId { get; set; }

        [Required]
        [Display(Name = "Option *")]
        public int QuestionId { get; set; }

        [Display(Name = "Answer")]
        public string Answer { get; set; }

        [Required]
        [Display(Name = "Right Answer *")]
        public bool IsRightAnswer { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Question Question { get; set; }
        public virtual QuestionOption QuestionOption { get; set; }
    }
}
