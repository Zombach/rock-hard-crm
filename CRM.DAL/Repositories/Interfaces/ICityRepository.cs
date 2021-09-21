using CRM.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.DAL.Repositories
{
    public interface ICityRepository
    {
        Task<List<CityDto>> GetAllCitiesAsync();
    }
}