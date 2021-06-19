using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// this table is used to short the many to many relation between group and Tasks 
/// 
/// </summary>
namespace DBContext.Models
{
    [Table("QuizIndex")]
    public partial class QuizIndex : BaseEntity
    {
        //[Required]
        //[Display(Name = "Quiz *")]
        //public int QuizId { get; set; }

        [Required]
        [Display(Name = "Customer *")]
        public int CustomerId { get; set; }

        [Display(Name = "Index")]
        public int Index { get; set; }  

        [Required]
        [Display(Name = "AppTask *")]
        public int AppTaskId { get; set; }

        public virtual AppTask Apptask { get; set; }    

        public virtual Customer Customer { get; set; }
    }
}
