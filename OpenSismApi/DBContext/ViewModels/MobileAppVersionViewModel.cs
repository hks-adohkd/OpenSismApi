namespace DBContext.ViewModels
{
    public partial class MobileAppVersionViewModel : BaseEntityViewModel
    {
        public bool IsCurrent { get; set; }
        public bool IsSupported { get; set; }
        public string VersionNumber { get; set; }
        public string VersionCode { get; set; }
        public string StoreUrl { get; set; }
        public string SyriaStoreUrl { get; set; }
        public string PlatformType { get; set; }
        public string Qrcode { get; set; }
        public string UpdateMsg { get; set; }
        public int NumberOfDownloads { get; set; }
        public string Note { get; set; }
    }
}
