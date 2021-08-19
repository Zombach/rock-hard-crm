using CRM.DAL.Enums;
using System.Collections.Generic;

namespace CRM.API.Models
{
    public class LeadInfoOutputModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string RegistrationDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsDeleted { get; set; }
        public List<AccountOutputModel> Accounts { get; set; }
        public Role Role { get; set; }
    }
}
