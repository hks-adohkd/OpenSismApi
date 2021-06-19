using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// this table to deefine the information about new task in App 
/// </summary>
namespace DBContext.Models
{
    [Table("AppTask")]
    public partial class AppTask : BaseEntity
    {
        public AppTask()
        {
            CustomerTasks = new HashSet<CustomerTask>();
            AppTaskGroups = new HashSet<AppTaskGroup>();
            QuizIndexs = new HashSet<QuizIndex>();
        }

        [NotMapped]
        public IFormFile file { get; set; }
        
        [NotMapped]
        public IFormFile vedioFile { get; set; }

        [Display(Name = "Image *")]
        public string ImageUrl { get; set; }

        [NotMapped]
        public string _DisplayName;
        [NotMapped]
        public string _DisplayNameAr;

        [Required]
        [Display(Name = "Task Arabic Name *")]
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
        [Display(Name = "Task English Name *")]
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

        [Display(Name = "Link/Url")]
        public string Link { get; set; }

        [Display(Name = "TutorialLink/Url")]
        public string TutorialLink { get; set; }

        [Required]  
        [Display(Name = "Start Date *")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date *")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Points *")]
        public int Points { get; set; }

        [Required]
        [Display(Name = "Users Limit *")]
        public int Limit { get; set; }

        [Required]
        [Display(Name = "Is Special *")]
        public bool Stared { get; set; }

        [Required]
        [Display(Name = "Type *")]
        public int? TaskTypeId { get; set; }  // to define the type of task (Quz, youtube, facebook ...etc)

        [Display(Name = "Package Name *")]
        public string PackageName { get; set; }

        [Display(Name = "Page Id *")]
        public string PageId { get; set; }

        [Display(Name = "Channel Id *")]
        public string VedioId { get; set; }
        
        [Display(Name = "Vedio Duration *")]
        public int? VedioDuration { get; set; }

        [Required]
        [Display(Name = "For All Groups")]
        public bool IsForAll { get; set; }

        public virtual SportMatch SportMatch { get; set; }
        public virtual TaskType TaskType { get; set; }

        [Display(Name = "Customers")]  // how many people solve this task , for statistics
        public virtual ICollection<CustomerTask> CustomerTasks { get; set; }
        public virtual ICollection<AppTaskGroup> AppTaskGroups { get; set; }

        public virtual ICollection<Quiz> Quizs { get; set; }    
           public virtual ICollection<QuizIndex> QuizIndexs { get; set; }   
    }
}
