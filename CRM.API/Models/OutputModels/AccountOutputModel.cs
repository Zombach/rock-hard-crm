using CRM.Business.Models;
using CRM.DAL.Enums;
using System.Collections.Generic;

namespace CRM.API.Models
{
    public class AccountOutputModel
    {
        public int Id { get; set; }
        public int LeadId { get; set; }
        public Currency Currency { get; set; }
        public string CreatedOn { get; set; }
        public decimal Balance { get; set; }
        public List<TransactionBusinessModel> Transactions { get; set; }
    }
}