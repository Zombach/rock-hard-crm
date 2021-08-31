using CRM.DAL.Models;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public interface ICityService
    {
        List<CityDto> GetAllCities();
    }
}