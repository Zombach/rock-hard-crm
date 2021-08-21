using CRM.DAL.Enums;
using CRM.DAL.Models;
using System;
using System.Collections.Generic;

namespace CRM.Business
{
    public class LeadFilterModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public List<Role> Roles { get; set; }
        public List<CityDto> Cities { get; set; }
        public DateTime? BirthDateFrom { get; set; }
        public DateTime? BirthDateTo { get; set; }
    }
}