using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public async Task<List<CityDto>> GetAllCities()
        {
            return await _cityRepository.GetAllCitiesAsync();
        }
    }
}