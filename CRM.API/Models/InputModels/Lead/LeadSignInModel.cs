using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class LeadSignInModel
    {
        [Required(ErrorMessage = EmailRequired)]
        [EmailAddress(ErrorMessage = WrongEmailFormat)]
        public string Email { get; set; }

        [Required(ErrorMessage = PasswordRequired)]
        [MinLength(8, ErrorMessage = WrongFormatPassword)]
        public string Password { get; set; }
    }
}