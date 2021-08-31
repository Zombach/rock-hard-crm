using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class TransferInputModel : TransactionInputModel
    {
        [Required(ErrorMessage = RecipientAccountRequired)]
        public int RecipientId { get; set; }
    }
}