using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class CommissionFeeShortOutputModel
    {
        public int LeadId { get; set; }
        public int AccountId { get; set; }
        public long TransactionId { get; set; }
        public Role Role { get; set; }
        public decimal Amount { get; set; }
    }
}