using CRM.DAL.Enums;
using static CRM.API.Common.ValidationMessage;
using System.Collections.Generic;
using DevEdu.API.Common;

namespace CRM.API.Models
{
    public class LeadFilterInputModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public List<Role> Roles { get; set; }
        public List<CityInputModel> Cities { get; set; }

        [CustomDateFormatAttribute(ErrorMessage = WrongDateFormat)]
        public string BirthDateFrom { get; set; }

        [CustomDateFormatAttribute(ErrorMessage = WrongDateFormat)]
        public string BirthDateTo { get; set; }
    }
}
