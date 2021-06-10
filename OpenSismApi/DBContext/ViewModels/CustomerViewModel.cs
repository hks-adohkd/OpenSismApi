using System;

namespace DBContext.ViewModels
{
    public partial class CustomerViewModel : BaseEntityViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageUrl { get; set; }
        public string Address { get; set; }
        public bool? TermsAndConditions { get; set; }
        public bool? Gender { get; set; }
        public int CurrentPoints { get; set; }
        public int TotalPoints { get; set; }
        public bool Premium { get; set; }
        public DateTime? LuckyWheelLastSpinDate { get; set; }
        public DateTime? LuckyWheelPremiumLastSpinDate { get; set; }
        public DateTime? DailyBonusLastUseDate { get; set; }
        public int? DailyBonusLevel { get; set; }
        public int? CityId { get; set; }
        public string UserId { get; set; }
        public bool LockoutEnabled { get; set; }
        public int GroupId { get; set; }
        public string FCMToken { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpiration { get; set; }
        public string FacbookFirstName { get; set; }
        public string FacbookLastName { get; set; }
        public string FacbookEmail { get; set; }
        public string FacbookId { get; set; }
        public string FacbookAccessToken { get; set; }
        public int NextGroupPoints { get; set; }

        public string ShareCode { get; set; }
        public string InstalledFrom { get; set; }

        public virtual GroupViewModel Group { get; set; }
        public virtual ApplicationUserViewModel User { get; set; }
        public virtual CityViewModel City { get; set; }
    }
}
