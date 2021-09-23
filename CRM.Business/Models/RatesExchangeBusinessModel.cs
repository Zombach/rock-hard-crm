using System.Collections.Generic;

namespace CRM.Business.Models
{
    public class RatesExchangeBusinessModel
    {
        public string Updated { get; set; }
        public string BaseCurrency { get; set; }
        public Dictionary<string, decimal> Rates { get; set; }
    }
}