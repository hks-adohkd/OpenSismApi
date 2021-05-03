using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("PrizeType")]
    public partial class PrizeType : BaseEntity
    {
        public PrizeType()
        {
            Prizes = new HashSet<Prize>();
        }

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

        public virtual ICollection<Prize> Prizes { get; set; }
    }
}
