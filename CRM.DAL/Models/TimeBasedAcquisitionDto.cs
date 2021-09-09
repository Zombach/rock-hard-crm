using System;
using CRM.DAL.Enums;

namespace CRM.DAL.Models
{
    public class TimeBasedAcquisitionDto
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int? LeadId { get; set; }
        public int? AccountId { get; set; }
        public Role Role { get; set; }
    }
}