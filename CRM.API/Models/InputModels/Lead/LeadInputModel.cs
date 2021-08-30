using CRM.API.Common;
using CRM.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class LeadInputModel
    {
        [Required(ErrorMessage = FirstNameRequired)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = LastNameRequired)]
        public string LastName { get; set; }

        [Required(ErrorMessage = PatronymicRequired)]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = EmailRequired)]
        [EmailAddress(ErrorMessage = WrongEmailFormat)]
        public string Email { get; set; }

        [Required(ErrorMessage = PhoneNumberRequired)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = PasswordRequired)]
        [MinLength(8, ErrorMessage = WrongFormatPassword)]
        public string Password { get; set; }

        [Required(ErrorMessage = BirthDateRequired)]
        [CustomDateFormat(ErrorMessage = WrongFormatBirthDate)]
        public string BirthDate { get; set; }

        public Role Role { get; set; }
        public int CityId { get; set; }
    }
}