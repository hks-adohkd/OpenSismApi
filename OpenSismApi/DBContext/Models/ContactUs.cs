using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// messages  using contact us in UI , when customer send us a message
/// </summary>
namespace DBContext.Models
{
    [Table("ContactUs")]
    public partial class ContactUs : BaseEntity
    {
        [Required]
        [Display(Name = "Phone Number *")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "First Name *")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name *")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Subject *")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Message *")]
        public string Message { get; set; }

        [Display(Name = "Featured Subject (FAQ Title)")]
        public string FeaturedSubject { get; set; }

        [Required]
        [Display(Name = "Is Featured (Enable it to make it FAQ) *")]
        public bool IsFeatured { get; set; }

        [Display(Name = "Reply")]
        public string Reply { get; set; }

        [Display(Name = "Featured Reply")]
        public string FeaturedReply { get; set; }

        [Required]
        [Display(Name = "Is Viewed *")]
        public bool IsViewed { get; set; }

        //Read replay by customer 
        [Display(Name = "Is Readed *")]
        public bool IsReaded { get; set; }

        [Display(Name = "Customer *")]
        public int? CustomerId { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
