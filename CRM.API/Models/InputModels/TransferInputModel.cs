using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class TransferInputModel : TransactionInputModel
    {
        [Required(ErrorMessage = RecipientAccountRequired)]
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = WrongFormatRecipientAccount)]
        public int RecipientAccountId { get; set; }
    }
}