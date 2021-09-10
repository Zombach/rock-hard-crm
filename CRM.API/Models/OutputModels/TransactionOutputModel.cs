using System;
using CRM.DAL.Enums;

namespace CRM.API.Models.OutputModels
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
