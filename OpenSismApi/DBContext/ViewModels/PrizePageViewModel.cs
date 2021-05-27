using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBContext.ViewModels
{
    public partial class PrizePageViewModel
    {
        public virtual List<PrizeViewModel> Prizes { get; set; }
    }
}

