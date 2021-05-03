using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// about us class 
/// </summary>
namespace DBContext.Models
{
    [Table("Contact")]
    public partial class Contact : BaseEntity
    {
        [NotMapped]
        public IFormFile file { get; set; }

        [Display(Name = "Icon")]
        public string Icon { get; set; }

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
        public string _Value;
        [NotMapped]
        public string _ValueAr;

        [Required]
        [Display(Name = "Arabic Value *")]
        public string ValueAr
        {
            get
            {
                return _ValueAr;
            }
            set
            {
                _ValueAr = value;
            }
        }

        [Required]
        [Display(Name = "English Value *")]
        public string Value
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _ValueAr;
                }
                else
                {
                    return _Value;
                }
            }
            set
            {
                _Value = value;
            }
        }
    }
}
