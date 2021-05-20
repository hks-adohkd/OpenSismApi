using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// messages  using contact us in UI , when customer send us a message
/// </summary>
namespace DBContext.Models
{
    [Table("Mail")]
    public partial class Mail : BaseEntity
    {
        
        

        

        [Required]
        [Display(Name = "Reciever Email")]
        public string RecieverEmail { get; set; }

        [Required]
        [Display(Name = "Subject *")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Message *")]
        public string Message { get; set; }



       
    }
}
