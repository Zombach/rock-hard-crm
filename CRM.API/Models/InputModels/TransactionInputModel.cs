using System;

namespace CRM.API.Models
{
    public class TransactionInputModel
    {
        public int AccountId { get; set; }
        public int TransactionType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}