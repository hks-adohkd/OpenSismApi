using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DBContext.Models
{
    [Table("Quiz")]
    public partial class Quiz : BaseEntity
    {
        public Quiz()   
        {
            QuizOptions = new HashSet<QuizOption>();
            //QuizIndexs = new HashSet<QuizIndex>();
        }

        [NotMapped]
        public string _Script;
        [NotMapped]
        public string _ScriptAr;

        [Required]
        [Display(Name = "Quiz Arabic Script *")]
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
        [Display(Name = "Quiz English Script *")]
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
        [Display(Name = "With Options *")]
        public bool Type { get; set; }

        [Required]
        [Display(Name = "Order *")]
        public int ItemOrder { get; set; }

        [Required]
        [Display(Name = "Task *")]
        public int AppTaskId { get; set; }


        public virtual AppTask AppTask { get; set; }
       // public virtual ICollection<CustomerAnswer> CustomerAnswers { get; set; }
        public virtual ICollection<QuizOption> QuizOptions { get; set; }

        //public virtual ICollection<QuizIndex> QuizIndexs { get; set; }
    }   
}
