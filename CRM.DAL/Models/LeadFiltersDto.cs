using CRM.DAL.Enums;
using System;
using System.Collections.Generic;

namespace CRM.DAL.Models
{
    public class LeadFiltersDto
    {
        public string FirstName { get; set; }
        public SearchType? SearchTypeForFirstName { get; set; }
        public string LastName { get; set; }
        public SearchType? SearchTypeForLastName { get; set; }
        public string Patronymic { get; set; }
        public SearchType? SearchTypeForPatronymic { get; set; }
        public List<int> Role { get; set; }
        public List<int> City { get; set; }
        public DateTime? BirthDateFrom { get; set; }
        public DateTime? BirthDateTo { get; set; }
    }
}
