using System;
using CRM.DAL.Enums;

namespace CRM.DAL.Models
{
    public class CommissionFeeDto
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