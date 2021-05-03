using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("CustomerTask")]
    public partial class CustomerTask : BaseEntity
    {
        [Required]
        [Display(Name = "Customer *")]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Task *")]
        public int AppTaskId { get; set; }

        [Required]
        [Display(Name = "Is Done *")]
        public bool IsDone { get; set; }

        [Display(Name = "Done Date")]
        public DateTime? DoneDate { get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }
        
        [Display(Name = "Earned Points")]
        public int EarnedPoints { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual AppTask AppTask { get; set; }
    }
}
