using System.ComponentModel.DataAnnotations;

namespace OpenSismApi.Models
{
    public class ResendCodeModel
    {
        [Required]
        public string Username { get; set; }

    }
}
