using CRM.Business.Services;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using SqlKata;

namespace CRM.Business.Extensions
{
    public static class LeadServiceExtensions
    {
        public static Query FilterByName(this LeadService service, 
            Query query, string partOfName, SearchType? searchType, string field)
        {
            if (partOfName is null) return query;

            return query.Where(
                    "l." + field,
                    BuildStringForCompare(searchType),
                    CreateStringBySearchingType(searchType, partOfName));
        }

        public static Query FilterByRole(this LeadService service, Query query, LeadFiltersDto filter)
        {
            return filter.Role is null ? query : query.Where("l.Role", filter.Role);
        }

        public static Query FilterByCity(this LeadService service, Query query, LeadFiltersDto filter)
        {
            return filter.City is null || filter.City.Count == 0 ? query : query.WhereIn("City.Id", filter.City);
        }

        public static Query FilterByBirthDate(this LeadService service, Query query, LeadFiltersDto filter)
        {
            query = filter.BirthDateFrom is null ? query : query.WhereDate("l.BirthDate", ">", filter.BirthDateFrom.ToString());
            query = filter.BirthDateTo is null ? query : query.WhereDate("l.BirthDate", "<", filter.BirthDateTo.ToString());
            return query;
        }

        private static string CreateStringBySearchingType(SearchType? searchType, string searchingString)
        {
            return searchType == SearchType.StartsWith ? searchingString + "%" :
                searchType == SearchType.Contains ? "%" + searchingString + "%" :
                searchType == SearchType.EndsWith ? "%" + searchingString : searchingString;
        }

        private static string BuildStringForCompare(SearchType? searchType)
        {
            return searchType == SearchType.Equals ? "=" :
                searchType == SearchType.NotEquals ? "!=" : "like";
        }
    }
}
