using CRM.DAL.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Business.Models
{
    public class AccountBusinessModel
    {
        private const string _recipientId = @"$.[?(@.RecipientAccountId)]";

        public int Id { get; set; }
        public int LeadId { get; set; }
        public Currency Currency { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? Closed { get; set; }
        public bool IsDeleted { get; set; }
        public List<TransactionBusinessModel> Transactions { get; set; }
        public List<TransferBusinessModel> Transfers { get; set; }
        public decimal Balance { get; set; }

        

        public static void GetListModels(string json)
        {
            var jArray = JArray.Parse(json);
            AccountBusinessModelExtension.Transfers = jArray.Where(j => j.SelectToken(_recipientId) != null)
                .Select(t => t.ToObject<TransferBusinessModel>()).ToList();
            AccountBusinessModelExtension.Transactions = jArray.Where(j => j.SelectToken(_recipientId) == null)
                .Select(t => t.ToObject<TransactionBusinessModel>()).ToList();
        }
    }
}