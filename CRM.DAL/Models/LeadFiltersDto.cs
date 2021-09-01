using CRM.DAL.Enums;
using System;
using System.Collections.Generic;

namespace CRM.DAL.Models
{
    public class LeadFiltersDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public SearchType SearchType { get; set; }
        public int? Role { get; set; }
        public List<int> City { get; set; }
        public DateTime? BirthDateFrom { get; set; }
        public DateTime? BirthDateTo { get; set; }
    }
}
