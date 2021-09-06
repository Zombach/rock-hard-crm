using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class TransactionInputModel
    {
        [Required(ErrorMessage = AccountRequired)]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = WrongFormatAccount)]
        public int AccountId { get; set; }

        [Required(ErrorMessage = AmountRequired)]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = WrongFormatAmount)]
        public decimal Amount { get; set; }
    }
}