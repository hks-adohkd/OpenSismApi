using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("Prize")]
    public partial class Prize : BaseEntity
    {
        public Prize()
        {
            CustomerPrizes = new HashSet<CustomerPrize>();
        }

        [NotMapped]
        public IFormFile file { get; set; }

        [Display(Name = "Image *")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Programmatically Name *")]
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

        [NotMapped]
        public string _Description;
        [NotMapped]
        public string _DescriptionAr;

        [Display(Name = "Arabic Description")]
        public string DescriptionAr
        {
            get
            {
                return _DescriptionAr;
            }
            set
            {
                _DescriptionAr = value;
            }
        }

        [Display(Name = "English Description")]
        public string Description
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _DescriptionAr;
                }
                else
                {
                    return _Description;
                }
            }
            set
            {
                _Description = value;
            }
        }

        [Required]
        [Display(Name = "Points *")]
        public int Points { get; set; }

        [Required]
        [Display(Name = "Type *")]
        public int PrizeTypeId { get; set; }

      //  [Display(Name = "Lucky Wheel")]
      //  public int? LuckyWheelId { get; set; }
        
     //   [Display(Name = "Daily Bonus")]
     //   public int? DailyBonusId { get; set; }

        [Required]
        [Display(Name = "Is Valid *")]
        public bool IsValid { get; set; }

        [Required]
        [Display(Name = "Order *")]
        public int ItemOrder { get; set; }

        public virtual PrizeType PrizeType { get; set; }
    //    public virtual LuckyWheel LuckyWheel { get; set; }
    //    public virtual DailyBonus DailyBonus { get; set; }

        [Display(Name = "Customers")]
        public virtual ICollection<CustomerPrize> CustomerPrizes { get; set; }
    }
}
