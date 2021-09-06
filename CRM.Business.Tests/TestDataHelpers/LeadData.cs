using CRM.DAL.Enums;
using CRM.DAL.Models;
using System.Collections;

namespace CRM.Business.Tests.Data
{
    public static class LeadData
    {
        public static IEnumerator GetLeadFilterData()
        {
            yield return new LeadFiltersDto { 
                SearchTypeForLastName = SearchType.StartsWith,
                LastName = "Ma"            
            };
            
        }
    }
}
