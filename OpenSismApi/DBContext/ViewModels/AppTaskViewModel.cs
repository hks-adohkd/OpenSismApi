using System;
using System.Collections.Generic;

namespace DBContext.ViewModels
{
    public partial class AppTaskViewModel : BaseEntityViewModel
    {
        public string ImageUrl { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool IsDone { get; set; }
        public bool IsReachLimit { get; set; }
        public string Link { get; set; }
        public string TutorialLink { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Points { get; set; }
        public int Limit { get; set; }
        public bool Stared { get; set; }
        public int? TaskTypeId { get; set; }
        public string PackageName { get; set; }
        public string PageId { get; set; }
        public string VedioId { get; set; }
        public int? VedioDuration { get; set; }
        public bool IsForAll { get; set; }

        public virtual TaskTypeViewModel TaskType { get; set; }
    }
}
