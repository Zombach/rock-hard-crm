using CRM.DAL.Enums;
using static CRM.API.Common.ValidationMessage;
using System.Collections.Generic;
using CRM.API.Common;

namespace CRM.API.Models
{
    public class LeadFiltersInputModel
    {
        public SearchType? SearchTypeForFirstName { get; set; } 
        public string FirstName { get; set; } // Masha
        public SearchType? SearchTypeForLastName { get; set; }
        public string LastName { get; set; } // Nastya
        public SearchType? SearchTypeForPatronymic { get; set; }
        public string Patronymic { get; set; } // Masha
        public int? Role { get; set; } // Nastya
        public List<int> City { get; set; } // Masha

        [CustomDateFormat(ErrorMessage = WrongFormatDate)]
        public string BirthDateFrom { get; set; } // Nastya

        [CustomDateFormat(ErrorMessage = WrongFormatDate)]
        public string BirthDateTo { get; set; } // Nasta
    }
}
