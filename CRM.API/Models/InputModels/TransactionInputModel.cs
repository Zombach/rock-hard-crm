using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class TransactionInputModel
    {
        public int AccountId { get; set; }
        public int RecipientId { get; set; }

        [Required(ErrorMessage = AmountRequired)]
        public decimal Amount { get; set; }
    }
}