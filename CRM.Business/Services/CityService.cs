using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;

        public CityService(ICityRepository cityRepository)
        {
            _cityRepository = cityRepository;
        }

        public List<CityDto> GetAllCities()
        {
            return _cityRepository.GetAllCities();
        }
    }
}