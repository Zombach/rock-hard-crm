using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class LeadFilterInputModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public Role? Role { get; set; }
        public CityInputModel City { get; set; }
        public string BirthDateFrom { get; set; }
        public string BirthDateTo { get; set; }
    }
}
