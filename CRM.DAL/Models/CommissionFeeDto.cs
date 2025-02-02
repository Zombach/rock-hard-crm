﻿using CRM.DAL.Enums;
using System;

namespace CRM.DAL.Models
{
    public class CommissionFeeDto
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public int AccountId { get; set; }
        public long TransactionId { get; set; }
        public TransactionType TransactionType { get; set; }
        public Role Role { get; set; }
        public DateTime Date { get; set; }
        public decimal CommissionAmount { get; set; }
    }
}