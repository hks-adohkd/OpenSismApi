using System;

namespace DBContext.ViewModels
{
    public partial class CustomerTaskViewModel : BaseEntityViewModel
    {
        public int CustomerId { get; set; }
        public int AppTaskId { get; set; }
        public bool IsDone { get; set; }
        public DateTime? DoneDate { get; set; }
        public DateTime? StartDate { get; set; }
        public string ShareCode { get; set; }
        public int EarnedPoints { get; set; }

        public virtual AppTaskViewModel AppTask { get; set; }
        public virtual SportMatchViewModel SportMatch { get; set; }
    }
}
