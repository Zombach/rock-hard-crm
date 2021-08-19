using CRM.DAL.Models;
using System.Collections.Generic;

namespace CRM.DAL.Repositories
{
    public interface ICityRepository
    {
        List<CityDto> GetAllCities();
    }
}
