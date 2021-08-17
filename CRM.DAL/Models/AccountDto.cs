using CRM.DAL.Enums;
using System;

namespace CRM.DAL.Models
{
    public class AccountDto
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public Currency Currency { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Closed { get; set; }
        public bool IsDeleted { get; set; }
    }
}