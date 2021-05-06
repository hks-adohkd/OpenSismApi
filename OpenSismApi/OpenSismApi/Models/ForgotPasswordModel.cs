using System.ComponentModel.DataAnnotations;

namespace OpenSismApi.Models
{
    public class ForgotPasswordModel
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }
    }
}
