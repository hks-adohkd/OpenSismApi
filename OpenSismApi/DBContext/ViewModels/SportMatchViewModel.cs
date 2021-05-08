using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DBContext.ViewModels
{
    public partial class SportMatchViewModel : BaseEntityViewModel
    {
        public string FirstTeamName { get; set; }
        public string SecondTeamName { get; set; }
        public string FirstTeamFlag { get; set; }
        public string SecondTeamFlag { get; set; }
        public string Type { get; set; }
        public string League { get; set; }
        public int AppTaskId { get; set; }
        public DateTime MatchTime { get; set; }
        public int? FirstTeamScore { get; set; }
        public int? SecondTeamScore { get; set; }

        public virtual AppTaskViewModel AppTask { get; set; }
    }
}
