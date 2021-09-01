using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class LeadByFiltersOutputModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string BirthDate { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public CityOutputModel City { get; set; }
        public string RegistrationDate { get; set; }
    }
}
