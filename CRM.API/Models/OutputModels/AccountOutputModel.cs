using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class AccountOutputModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public Currency Currency { get; set; }
        public string CreatedOn { get; set; }
    }
}