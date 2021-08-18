using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class LeadInputModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public Role RoleId { get; set; }
        public CityInputModel City { get; set; }
    }
}