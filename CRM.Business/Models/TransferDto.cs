using System.Runtime.Serialization;
using CRM.DAL.Enums;

namespace CRM.Business.Models
{
    [KnownType(typeof(TransferBusinessModel))]
    public class TransferBusinessModel : TransactionBusinessModel
    {
        public int RecipientAccountId { get; set; }
        public decimal RecipientAmount { get; set; }
        public Currency RecipientCurrency { get; set; }
    }
}