using System;


/// <summary>
/// the messages of customer 
/// </summary>
namespace DBContext.ViewModels
{
    public partial class CustomerMessageViewModel : BaseEntityViewModel
    {
        public int CustomerId { get; set; }
        public int MessageId { get; set; }
        public bool IsRead { get; set; }
        public DateTime SendDate { get; set; }

        public virtual MessageViewModel Message { get; set; }
    }
}
