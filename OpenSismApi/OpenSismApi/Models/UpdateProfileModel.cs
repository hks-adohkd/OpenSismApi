using System.ComponentModel.DataAnnotations;

namespace OpenSismApi.Models
{
    public class UpdateProfileModel
    {

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "City is required")]
        public int CityId { get; set; }

        public string ImageUrl { get; set; }
    }

    public class UpdateFCMTokenModel
    {
        public string FCMToken { get; set; }
    }
}
