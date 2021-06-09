using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("CustomerPrize")]
    public partial class CustomerPrize : BaseEntity
    {
        [Required]
        [Display(Name = "Customer *")]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Prize *")]
        public int PrizeId { get; set; }

        [Display(Name = "Earn Date")]
        public DateTime EarnDate { get; set; }

        [Required]
        [Display(Name = "Request Date *")]
        public DateTime RequestDate { get; set; }

        [Required]
        [Display(Name = "Status *")]
        public int PrizeStatusId { get; set; }

        [Display(Name = "PrizeDescription")]
        public String Description { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Prize Prize { get; set; }
        public virtual PrizeStatus PrizeStatus { get; set; }
    }
}
