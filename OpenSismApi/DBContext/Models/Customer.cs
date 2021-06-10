using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("Customer")]
    public partial class Customer : BaseEntity
    {
        public Customer()
        {
            ContacstUs = new HashSet<ContactUs>();
            CustomerPrizes = new HashSet<CustomerPrize>();
            CustomerTasks = new HashSet<CustomerTask>();
            //CustomerAnswers = new HashSet<CustomerAnswer>();
            CustomerMessages = new HashSet<CustomerMessage>();
        }

        [Required]
        [Display(Name = "First Name *")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name *")]
        public string LastName { get; set; }

        [Display(Name = "Profile Image")]
        public string ImageUrl { get; set; }

        [Display(Name = " Address")]
        public string Address { get; set; }

        [Required]  
        [Display(Name = "Terms & Conditions *")]
        public bool? TermsAndConditions { get; set; }

        [Required]
        [Display(Name = "Gender *")]
        public bool? Gender { get; set; }

        [Required]
        [Display(Name = "Points *")]
        public int CurrentPoints { get; set; }

        [Required]
        [Display(Name = "Points *")]
        public int TotalPoints { get; set; }

        [Required]
        [Display(Name = "Premium *")]
        public bool? Premium { get; set; }

        [Required]
        [Display(Name = "Lucky Wheel Last Spin Date *")]
        public DateTime? LuckyWheelLastSpinDate { get; set; }

        
        [Display(Name = "Lucky Wheel Premium Last Spin Date *")]
        public DateTime? LuckyWheelPremiumLastSpinDate { get; set; }

        [Required]
        [Display(Name = "Daily Bonus Last Use Date")]
        public DateTime? DailyBonusLastUseDate { get; set; }
        
        [Required]
        [Display(Name = "Daily Bonus Level")]
        public int? DailyBonusLevel { get; set; }

       
        [Display(Name = "City *")]
        public int? CityId { get; set; }

        [Required]
        [Display(Name = "User *")]
        public string UserId { get; set; }

        [NotMapped]
        public bool LockoutEnabled { get; set; }

        [NotMapped]
        public int NextGroupPoints { get; set; }

        [Required]
        [Display(Name = "Group *")]
        public int GroupId { get; set; }

        [Display(Name = "FCM Token *")]
        public string FCMToken { get; set; }

        [Display(Name = "Token *")]
        public string Token { get; set; }

        [Display(Name = "Code")]
        public string ResetCode { get; set; }

        [Display(Name = "Token Expiration *")]
        public DateTime? TokenExpiration { get; set; }

        public string FacbookFirstName { get; set; }
        public string FacbookLastName { get; set; }
        public string FacbookEmail { get; set; }
        public string FacbookId { get; set; }
        public string FacbookAccessToken { get; set; }

        public string ShareCode { get; set; }
        public string InstalledFrom { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual City City { get; set; }
        [Display(Name = "Group")]
        public virtual Group Group { get; set; }

        public virtual ICollection<ContactUs> ContacstUs { get; set; }
        [Display(Name = "Prizes")]
        public virtual ICollection<CustomerPrize> CustomerPrizes { get; set; }
        [Display(Name = "Tasks")]
        public virtual ICollection<CustomerTask> CustomerTasks { get; set; }
       // public virtual ICollection<CustomerAnswer> CustomerAnswers { get; set; }
        public virtual ICollection<CustomerMessage> CustomerMessages { get; set; }

       

    }
}
