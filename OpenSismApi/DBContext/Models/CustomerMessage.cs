using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("CustomerMessage")]
    public partial class CustomerMessage : BaseEntity
    {
        [Required]
        [Display(Name = "Customer *")]
        public int CustomerId { get; set; }

        [Required]
        [Display(Name = "Message *")]
        public int MessageId { get; set; }

        [Required]
        [Display(Name = "Is Read *")]
        public bool IsRead { get; set; }

        [Required]
        [Display(Name = "Send Date *")]
        public DateTime SendDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Message Message { get; set; }
    }
}
