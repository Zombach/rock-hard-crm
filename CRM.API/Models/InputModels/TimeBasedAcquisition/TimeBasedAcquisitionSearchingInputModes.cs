using CRM.DAL.Enums;
using System.ComponentModel.DataAnnotations;
using static CRM.API.Common.ValidationMessage;

namespace CRM.API.Models
{
    public class TimeBasedAcquisitionSearchingInputModes : TimeBasedAcquisitionInputModel //не надо ли отдельно
    {
        [Range(minimum: 1, maximum: int.MaxValue, ErrorMessage = WrongFormatLeadId)]
        public int? LeadId { get; set; }
        public Role Role { get; set; }
    }
}