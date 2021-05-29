using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("DailyBonus")]
    public partial class DailyBonus : BaseEntity
    {
        public DailyBonus()
        {
            Prizes = new HashSet<Prize>();
        }

        [Required]
        [Display(Name = "Parts Count *")]
        public int PartsCount { get; set; }

        [Display(Name = "is Premium *")]
        public bool IsPremium { get; set; }

        public virtual ICollection<Prize> Prizes { get; set; }

    }
}
