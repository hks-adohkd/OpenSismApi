using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("MobileAppVersion")]
    public partial class MobileAppVersion : BaseEntity
    {
        [Required]
        [Display(Name = "Is Current *")]
        public bool IsCurrent { get; set; }

        [Required]
        [Display(Name = "Is Supported *")]
        public bool IsSupported { get; set; }

        [Required]
        [Display(Name = "Version Number *")]
        public string VersionNumber { get; set; }

        [Display(Name = "Version Code")]
        public string VersionCode { get; set; }

        [Required]
        [Display(Name = "Store Url *")]
        public string StoreUrl { get; set; }

        [Display(Name = "Syria Store Url")]
        public string SyriaStoreUrl { get; set; }

        [Required]
        [Display(Name = "Platform Type *")]
        public string PlatformType { get; set; }

        [Display(Name = "Qrcode")]
        public string Qrcode { get; set; }


        [NotMapped]
        public string _UpdateMsg;
        [NotMapped]
        public string _UpdateMsgAr;

        [Display(Name = "Arabic Update Message")]
        public string UpdateMsgAr
        {
            get
            {
                return _UpdateMsgAr;
            }
            set
            {
                _UpdateMsgAr = value;
            }
        }

        [Display(Name = "English Update Message")]
        public string UpdateMsg
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _UpdateMsgAr;
                }
                else
                {
                    return _UpdateMsg;
                }
            }
            set
            {
                _UpdateMsg = value;
            }
        }

        [Display(Name = "Number Of Downloads")]
        public int NumberOfDownloads { get; set; }


        [NotMapped]
        public string _Note;
        [NotMapped]
        public string _NoteAr;

        [Display(Name = "Arabic Note")]
        public string NoteAr
        {
            get
            {
                return _NoteAr;
            }
            set
            {
                _NoteAr = value;
            }
        }

        [Display(Name = "English Note")]
        public string Note
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _NoteAr;
                }
                else
                {
                    return _Note;
                }
            }
            set
            {
                _Note = value;
            }
        }
    }
}
