using System.Collections.Generic;

namespace DBContext.ViewModels
{
    public partial class DailyBonusViewModel : BaseEntityViewModel
    {
        public int PartsCount { get; set; }
        public bool IsDoneToday { get; set; }
        public bool IsDoneYesterday { get; set; }

        public virtual List<PrizeViewModel> Prizes { get; set; }

    }
}
