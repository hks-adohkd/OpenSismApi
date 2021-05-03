using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// this table is used to short the many to many relation between group and Tasks 
/// 
/// </summary>
namespace DBContext.Models
{
    [Table("AppTaskGroup")]
    public partial class AppTaskGroup : BaseEntity
    {
        [Required]
        [Display(Name = "Task *")]
        public int AppTaskId { get; set; }

        [Required]
        [Display(Name = "Group *")]
        public int GroupId { get; set; }

        public virtual AppTask AppTask { get; set; }
        public virtual Group Group { get; set; }
    }
}
