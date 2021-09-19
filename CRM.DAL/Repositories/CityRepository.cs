using CRM.Core;
using CRM.DAL.Models;
using Dapper;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.DAL.Repositories
{
    public class CityRepository : BaseRepository, ICityRepository
    {
        private const string _citySelectAll = "dbo.City_SelectAll";

        public CityRepository(IOptions<DatabaseSettings> options) : base(options) { }

        public async Task<List<CityDto>> GetAllCitiesAsync()
        {
            return (await _connection.QueryAsync<CityDto>(
                _citySelectAll,
                    commandType: CommandType.StoredProcedure
                ))
                .ToList();
        }
    }
}