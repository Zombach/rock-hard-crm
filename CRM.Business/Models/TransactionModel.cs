using System;
using CRM.DAL.Enums;

namespace CRM.Business.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }
        public TransactionType TransactionType { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}