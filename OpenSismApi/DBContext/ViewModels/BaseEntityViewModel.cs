using System;

namespace DBContext.ViewModels
{
    public class BaseEntityViewModel
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public bool IsDeleted { get; set; }
    }
}
