using CRM.DAL.Enums;
using CRM.DAL.Models;
using SqlKata;
using System.Collections;
using System.Collections.Generic;

namespace CRM.Business.Tests.Data
{
    public static class LeadData
    {
        public static IEnumerator GetLeadFilterData()
        {
            yield return new object[] {
                new LeadFiltersDto {
                    SearchTypeForLastName = SearchType.StartsWith,
                    LastName = "Ma"
                },
                new SqlResult()
                {
                    Sql = "Select l.Id, l.FirstName, l.LastName, l.Patronymic, l.Email, l.BirthDate, City.Id, City.Name, l.Role as Id from Lead as l " +
                    "where l.LastName like @p0 and l.IsDeleted = @p1 join City on City.Id = l.CityId",
                    NamedBindings = new Dictionary<string, object>()
                    {
                        { "@p0", "Ma"},
                        { "@p1", 0}
                    }
                }

            };

        }
    }
}
