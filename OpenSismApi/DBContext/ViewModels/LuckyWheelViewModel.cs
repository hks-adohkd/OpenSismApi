using System.Collections.Generic;

namespace DBContext.ViewModels
{
    public partial class LuckyWheelViewModel : BaseEntityViewModel
    {
        public int GroupId { get; set; }
        public int PartsCount { get; set; }

        public bool IsPremium { get; set; }
        public bool IsDoneToday { get; set; }

        public virtual GroupViewModel Group { get; set; }
        public virtual List<PrizeViewModel> Prizes { get; set; }

    }
}
