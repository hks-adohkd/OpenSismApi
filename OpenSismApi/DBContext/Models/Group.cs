using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// this table to identify the user group badge (silver gold premium etc...)
/// relation one to many with customer table 
/// </summary>

namespace DBContext.Models
{
    [Table("Group")]
    public partial class Group : BaseEntity
    {
        public Group()
        {
            Customers = new HashSet<Customer>();
           // LuckyWheels = new HashSet<LuckyWheel>();
          //  MessageGroups = new HashSet<MessageGroup>();
          //  AppTaskGroups = new HashSet<AppTaskGroup>();
        }

        [NotMapped]
        public IFormFile file { get; set; }

        [Display(Name = "Image *")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Programmatically Name")]
        public string Name { get; set; }

        [NotMapped]
        public string _DisplayName;
        [NotMapped]
        public string _DisplayNameAr;

        [Required]
        [Display(Name = "Arabic Name *")]
        public string DisplayNameAr
        {
            get
            {
                return _DisplayNameAr;
            }
            set
            {
                _DisplayNameAr = value;
            }
        }

        [Required]
        [Display(Name = "English Name *")]
        public string DisplayName
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _DisplayNameAr;
                }
                else
                {
                    return _DisplayName;
                }
            }
            set
            {
                _DisplayName = value;
            }
        }

        [Required]
        [Display(Name = "Order *")]
        public int ItemOrder { get; set; }

        [Required]
        [Display(Name = "Color *")]
        public string Color { get; set; }

        [Required]
        [Display(Name = "Points *")]
        public int Points { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
      //  public virtual ICollection<LuckyWheel> LuckyWheels { get; set; }
        public virtual ICollection<MessageGroup> MessageGroups { get; set; }
        public virtual ICollection<AppTaskGroup> AppTaskGroups { get; set; }
    }
}
