using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


/// <summary>
/// this class for slider 
/// </summary>
namespace DBContext.Models
{
    [Table("Content")]
    public partial class Content : BaseEntity
    {
        [Required]
        [Display(Name = "Programmatically Name *")]
        public string Name { get; set; }

        [NotMapped]
        public IFormFile file { get; set; }

        [Display(Name = "Image")]
        public string ImageUrl { get; set; }

        [NotMapped]
        public string _Title;
        [NotMapped]
        public string _TitleAr;

        [Display(Name = "Arabic Caption")]
        [AllowHtml]
        public string TitleAr
        {
            get
            {
                return _TitleAr;
            }
            set
            {
                _TitleAr = value;
            }
        }

        [Display(Name = "English Caption")]
        [AllowHtml]
        public string Title
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _TitleAr;
                }
                else
                {
                    return _Title;
                }
            }
            set
            {
                _Title = value;
            }
        }

        [NotMapped]
        public string _Script;
        [NotMapped]
        public string _ScriptAr;

        [Display(Name = "Arabic Script *")]
        [AllowHtml]
        public string ScriptAr
        {
            get
            {
                return _ScriptAr;
            }
            set
            {
                _ScriptAr = value;
            }
        }

        [Display(Name = "English Script *")]
        [AllowHtml]
        public string Script
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _ScriptAr;
                }
                else
                {
                    return _Script;
                }
            }
            set
            {
                _Script = value;
            }
        }

        [Required]
        [Display(Name = "Order *")]
        public int ItemOrder { get; set; }
    }
}
