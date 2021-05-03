using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("MessageGroup")]
    public partial class MessageGroup : BaseEntity
    {
        [Required]
        [Display(Name = "Message *")]
        public int MessageId { get; set; }

        [Required]
        [Display(Name = "Group *")]
        public int GroupId { get; set; }

        public virtual Message Message { get; set; }
        public virtual Group Group { get; set; }
    }
}
