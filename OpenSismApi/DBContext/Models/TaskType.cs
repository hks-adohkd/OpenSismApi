using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("TaskType")]
    public partial class TaskType : BaseEntity
    {
        public TaskType()
        {
            AppTasks = new HashSet<AppTask>();
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

        [Display(Name = "Tasks")]
        public virtual ICollection<AppTask> AppTasks { get; set; }
    }
}
