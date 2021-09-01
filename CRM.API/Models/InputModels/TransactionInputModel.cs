using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class TransactionInputModel
    {
        [Required(ErrorMessage = AccountRequired)]
        public int AccountId { get; set; }

        [Required(ErrorMessage = AmountRequired)]
        public decimal Amount { get; set; }
    }
}