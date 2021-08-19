using CRM.DAL.Models;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CRM.DAL.Repositories
{
    public class CityRepository : BaseRepository, ICityRepository
    {
        private const string _citySelectAll = "dbo.City_SelectAll"; 

        public List<CityDto> GetAllCities()
        {
            return _connection.Query<CityDto>(
                _citySelectAll,
                    commandType: CommandType.StoredProcedure
                )
                .ToList();
        }

    }
}
