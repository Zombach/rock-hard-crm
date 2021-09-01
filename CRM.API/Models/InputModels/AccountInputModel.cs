using CRM.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class AccountInputModel
    {
        [Required(ErrorMessage = CurrencyRequired)]
        public Currency Currency { get; set; }
    }
}