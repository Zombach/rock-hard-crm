using CRM.DAL.Enums;
using System;
using System.Collections.Generic;

namespace CRM.DAL.Models
{
    public class LeadDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public int BirthYear { get; set; }
        public int BirthMonth { get; set; }
        public int BirthDay { get; set; }
        public bool IsDeleted { get; set; }
        public Role Role { get; set; }
        public CityDto City { get; set; }
        public List<AccountDto> Accounts { get; set; }        
    }
}