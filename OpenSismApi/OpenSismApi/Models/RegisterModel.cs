using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;

namespace OpenSismApi.Models
{
    public class RegisterModel
    {
        [Required]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Terms & Conditions agreement are required")]
        public bool TermsAndConditions { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "City is required")]
        public int CityId { get; set; }

        public string FCMToken { get; set; }

    }

    public class SyrianNumberLenghtAttribute : ValidationAttribute
    {
        public int Length { get; set; }

        public SyrianNumberLenghtAttribute()
        {
            Length = 9;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _localizationService = (IStringLocalizer<RegisterModel>)validationContext
                .GetService(typeof(IStringLocalizer<RegisterModel>));
            string strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                int len = strValue.Length;
                if (len == this.Length)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(_localizationService["PhoneNumberLenth"]);
            }
            return ValidationResult.Success;
        }
    }

    public class SyrianNumberFormatAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _localizationService = (IStringLocalizer<RegisterModel>)validationContext
                .GetService(typeof(IStringLocalizer<RegisterModel>));
            string strValue = value as string;
            if (!string.IsNullOrEmpty(strValue))
            {
                var isNumeric = int.TryParse(strValue, out int n);
                if (strValue.StartsWith("9") && isNumeric)
                    return ValidationResult.Success;
                else
                    return new ValidationResult(_localizationService["InvalidPhoneNumber"]);
            }
            return ValidationResult.Success;
        }
    }
}
