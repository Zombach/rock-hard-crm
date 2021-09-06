using CRM.DAL.Enums;
using System.Runtime.Serialization;

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