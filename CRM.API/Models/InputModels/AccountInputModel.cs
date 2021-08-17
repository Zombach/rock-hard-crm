using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class AccountInputModel
    {
        public int LeadId { get; set; }
        public Currency Currency { get; set; }
    }
}
