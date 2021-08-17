using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class LeadUpdateInputModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int CityId { get; set; }
    }
}