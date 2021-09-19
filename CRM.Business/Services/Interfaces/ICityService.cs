using CRM.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface ICityService
    {
        Task<List<CityDto>> GetAllCities();
    }
}