using CRM.DAL.Enums;

namespace CRM.Business.Models
{
    public class TransferBusinessModel : TransactionBusinessModel
    {
        public int RecipientAccountId { get; set; }
        public decimal RecipientAmount { get; set; }
        public Currency RecipientCurrency { get; set; }
    }
}