using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("LuckyWheel")]
    public partial class LuckyWheel : BaseEntity
    {
        public LuckyWheel()
        {
            Prizes = new HashSet<Prize>();
        }

        [Required]
        [Display(Name = "Group *")]
        public int GroupId { get; set; }

        [Required]
        [Display(Name = "Parts Count *")]
        public int PartsCount { get; set; }

        public virtual Group Group { get; set; }
        public virtual ICollection<Prize> Prizes { get; set; }

    }
}
