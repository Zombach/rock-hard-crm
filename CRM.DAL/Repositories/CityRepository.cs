using CRM.Core;
using CRM.DAL.Models;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CRM.DAL.Repositories
{
    public class CityRepository : BaseRepository, ICityRepository
    {
        private const string _citySelectAll = "dbo.City_SelectAll";

        public CityRepository(IOptions<DatabaseSettings> options) : base(options) { }

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