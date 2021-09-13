using CRM.API.Common;
using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class TimeBasedAcquisitionInputModel
    {
        [Required(ErrorMessage = DateFromRequired)]
        [CustomDateFormatWithHoursAndMinutes(ErrorMessage = WrongFormatDateWithHoursAndMinutes)]
        public string From { get; set; }

        [Required(ErrorMessage = DateToRequired)]
        [CustomDateFormatWithHoursAndMinutes(ErrorMessage = WrongFormatDateWithHoursAndMinutes)]
        public string To { get; set; }

        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = WrongFormatAccount)]
        public int? AccountId { get; set; }
    }
}