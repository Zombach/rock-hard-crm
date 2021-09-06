using System;
using CRM.DAL.Enums;

namespace CRM.API.Models
{
    public class CommissionFeeOutputModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public int AccountId { get; set; }
        public int TransactionId { get; set; }
        public Role Role { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}