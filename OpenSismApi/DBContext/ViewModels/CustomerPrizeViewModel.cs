using System;

namespace DBContext.ViewModels
{
    public partial class CustomerPrizeViewModel : BaseEntityViewModel
    {
        public int CustomerId { get; set; }
        public int PrizeId { get; set; }
        public DateTime EarnDate { get; set; }
        public DateTime RequestDate { get; set; }
        public int PrizeStatusId { get; set; }

        public virtual PrizeViewModel Prize { get; set; }
        public virtual PrizeStatusViewModel PrizeStatus { get; set; }
    }
}
