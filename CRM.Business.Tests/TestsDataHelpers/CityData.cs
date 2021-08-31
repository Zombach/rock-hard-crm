using System.Collections.Generic;
using CRM.DAL.Models;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public class CityData
    {
        public static List<CityDto> GetListCityDto()
        {
            return new List<CityDto>()
            {
                new()
                {
                    Id = 1,
                    Name = "saint-Petersburg"
                },
                new()
                {
                    Id = 2,
                    Name = "Moscow"
                },
                new()
                {
                    Id = 3,
                    Name = "Tokyo"
                }
            };
        }
    }
}