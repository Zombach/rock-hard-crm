using CRM.DAL.Enums;
using System.Collections.Generic;

namespace CRM.DAL.Models
{
    public class LeadFiltersDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public SearchType SearchType { get; set; }
        public List<Role> Roles { get; set; }
        public List<CityDto> Cities { get; set; }
        public string BirthDateFrom { get; set; }
        public string BirthDateTo { get; set; }
    }
}
