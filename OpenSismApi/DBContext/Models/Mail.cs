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
        
        [Display(Name = "Phone Number *")]
        public string PhoneNumber { get; set; }

        
        [Display(Name = "First Name *")]
        public string FirstName { get; set; }

        
        [Display(Name = "Last Name *")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Sender Email")]
        public string SenderEmail { get; set; }

        [Required]
        [Display(Name = "Reciever Email")]
        public string RecieverEmail { get; set; }

        [Required]
        [Display(Name = "Subject *")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Message *")]
        public string Message { get; set; }



        [Display(Name = "Customer *")]
        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
