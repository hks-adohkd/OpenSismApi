using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DBContext.Models
{
    [Table("QuestionOption")]
    public partial class QuestionOption : BaseEntity
    {
        public QuestionOption()
        {
            CustomerAnswers = new HashSet<CustomerAnswer>();
        }

        [NotMapped]
        public string _Script;
        [NotMapped]
        public string _ScriptAr;

        [Required]
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

        [Required]
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
        [Display(Name = "Question *")]
        public int QuestionId { get; set; }

        [Required]
        [Display(Name = "Order *")]
        public int ItemOrder { get; set; }

        [Required]
        [Display(Name = "Right Option *")]
        public bool IsRightOption { get; set; }

        public virtual Question Question { get; set; }
        public virtual ICollection<CustomerAnswer> CustomerAnswers { get; set; }
    }
}
