using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBContext.Models
{
    [Table("Message")]
    public partial class Message : BaseEntity
    {
        public Message()
        {
            MessageGroups = new HashSet<MessageGroup>();
            CustomerMessages = new HashSet<CustomerMessage>();
        }

        [NotMapped]
        public string _Title;
        [NotMapped]
        public string _TitleAr;

        [Required]
        [Display(Name = "Arabic Title *")]
        public string TitleAr
        {
            get
            {
                return _TitleAr;
            }
            set
            {
                _TitleAr = value;
            }
        }

        [Required]
        [Display(Name = "English Title *")]
        public string Title
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _TitleAr;
                }
                else
                {
                    return _Title;
                }
            }
            set
            {
                _Title = value;
            }
        }

        [NotMapped]
        public string _Script;
        [NotMapped]
        public string _ScriptAr;

        [Required]
        [Display(Name = "Arabic Script *")]
        public string ScriptAr
        {
            get
            {
                return _ScriptAr;
            }
            set
            {
                _ScriptAr = value;
            }
        }

        [Required]
        [Display(Name = "English Script *")]
        public string Script
        {
            get
            {
                if (OpenSismDBContext._culture == "ar")
                {
                    return _ScriptAr;
                }
                else
                {
                    return _Script;
                }
            }
            set
            {
                _Script = value;
            }
        }

        [Required]
        [Display(Name = "For All Users/Groups")]
        public bool IsForAll { get; set; }

        [Required]
        [Display(Name = "For Specific Customer")]
        public bool IsForCustomer { get; set; }

        public virtual ICollection<MessageGroup> MessageGroups { get; set; }
        public virtual ICollection<CustomerMessage> CustomerMessages { get; set; }
    }
}
