using CRM.DAL.Enums;
using System;

namespace CRM.API.Models
{
    public class TransactionOutputModel
    {
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int AccountId { get; set; }
        public string Currency { get; set; }
    }
}