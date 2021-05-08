using System.ComponentModel.DataAnnotations;

namespace OpenSismApi.Models
{
    public class VerifyModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Code { get; set; }

    }
}
