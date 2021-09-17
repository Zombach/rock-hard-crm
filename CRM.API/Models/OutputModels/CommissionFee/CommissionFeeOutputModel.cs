using System;

namespace CRM.API.Models
{
    public class CommissionFeeOutputModel : CommissionFeeShortOutputModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
    }
}