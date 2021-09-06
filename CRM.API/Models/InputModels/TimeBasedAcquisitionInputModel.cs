using System.ComponentModel.DataAnnotations;
using CRM.API.Common;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class TimeBasedAcquisitionInputModel
    {
        [Required(ErrorMessage = DateFromRequired)]
        [CustomDateFormat(ErrorMessage = WrongFormatDate)]
        public string From { get; set; }
        [Required(ErrorMessage = DateToRequired)]
        [CustomDateFormat(ErrorMessage = WrongFormatDate)]
        public string To { get; set; }
    }
}