using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("UserLog")]
    public partial class UserLog : BaseEntity
    {
        [Display(Name = "Customer")]
        public int? CustomerId { get; set; }

        public string Platform { get; set; }

        [Display(Name = "Language")]
        public string AcceptLanguage { get; set; }

        [Display(Name = "App Version")]
        public string AppVersion { get; set; }

        [Display(Name = "OS Version")]
        public string OsVersion { get; set; }

        [Display(Name = "Mobile Brand")]
        public string MobileBrand { get; set; }

        [Display(Name = "Unique Device Id")]
        public string DeviceId { get; set; }

        public string Token { get; set; }

        public string Action { get; set; }

        public string Status { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
