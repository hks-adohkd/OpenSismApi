using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DBContext.Models
{
    [Table("SportMatch")]
    public partial class SportMatch : BaseEntity
    {
        public SportMatch()
        {
            CustomerPredictions = new HashSet<CustomerPrediction>();
        }

        [NotMapped]
        public IFormFile FirstFile { get; set; }

        [NotMapped]
        public IFormFile SecondFile { get; set; }

        [NotMapped]
        public string _FirstTeamName;
        [NotMapped]
        public string _FirstTeamNameAr;

        [Display(Name = "First Team Arabic Name *")]
        public string FirstTeamNameAr
        {
            get
            {
                return _FirstTeamNameAr;
            }
            set
            {
                _FirstTeamNameAr = value;
            }
        }

        [Display(Name = "First Team English Name *")]
        public string FirstTeamName
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _FirstTeamNameAr;
                }
                else
                {
                    return _FirstTeamName;
                }
            }
            set
            {
                _FirstTeamName = value;
            }
        }

        [NotMapped]
        public string _SecondTeamName;
        [NotMapped]
        public string _SecondTeamNameAr;

        [Display(Name = "Second Team Arabic Name *")]
        public string SecondTeamNameAr
        {
            get
            {
                return _SecondTeamNameAr;
            }
            set
            {
                _SecondTeamNameAr = value;
            }
        }

        [Display(Name = "Second Team English Name *")]
        public string SecondTeamName
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _SecondTeamNameAr;
                }
                else
                {
                    return _SecondTeamName;
                }
            }
            set
            {
                _SecondTeamName = value;
            }
        }

        [Display(Name = "First Team Flag *")]
        public string FirstTeamFlag { get; set; }

        [Display(Name = "Second Team Flag *")]
        public string SecondTeamFlag { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "League")]
        public string League { get; set; }

        [Required]
        [Display(Name = "Task *")]
        public int AppTaskId { get; set; }

        [Display(Name = "Match Date/Time *")]
        public DateTime? MatchTime { get; set; }

        [Display(Name = "First Team Score")]
        public int? FirstTeamScore { get; set; }

        [Display(Name = "Second Team Score")]
        public int? SecondTeamScore { get; set; }

        public virtual AppTask AppTask { get; set; }
        public virtual ICollection<CustomerPrediction> CustomerPredictions { get; set; }
    }
}
