namespace DBContext.ViewModels
{
    public partial class PrizeViewModel : BaseEntityViewModel
    {
        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public int Points { get; set; }
        public int Value { get; set; }
        public int PrizeTypeId { get; set; }
        public int? DailyBonusId { get; set; }
        public int? LuckyWheelId { get; set; }
        public bool IsValid { get; set; }
        public int ItemOrder { get; set; }

        public virtual PrizeTypeViewModel PrizeType { get; set; }
    }
}
