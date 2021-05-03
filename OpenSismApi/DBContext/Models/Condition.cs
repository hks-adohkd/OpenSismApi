using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// this table to configure some conditions in UI (slider time , .... )
/// </summary>
namespace DBContext.Models
{
    [Table("Condition")]
    public partial class Condition : BaseEntity
    {
        [Required]
        [Display(Name = "Programmatically Name *")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Name *")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Value *")]
        public string Value { get; set; }

        [Required]
        [Display(Name = "Is Valid *")]
        public bool IsValid { get; set; }
    }
}
