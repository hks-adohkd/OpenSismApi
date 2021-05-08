using System.Collections.Generic;

namespace DBContext.ViewModels
{
    public partial class IntroViewModel
    {
        public virtual List<ContentViewModel> IntroImages { get; set; }
        public virtual ContentViewModel IntroVideo { get; set; }
    }
}
