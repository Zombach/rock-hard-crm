using AutoMapper;
using CRM.API.Models;
using CRM.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CRM.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICityService _cityService;

        public DictionaryController(IMapper mapper, ICityService cityService)
        {
            _mapper = mapper;
            _cityService = cityService;
        }

        // api/dictionary/city
        [HttpGet("city")]
        [Description("Return list cities")]
        [ProducesResponseType(typeof(List<CityOutputModel>), StatusCodes.Status200OK)]
        public async Task<List<CityOutputModel>> GetAllCitiesAsync()
        {
            var listDto = await _cityService.GetAllCities();
            var listOutPut = _mapper.Map<List<CityOutputModel>>(listDto);
            return listOutPut;
        }
    }
}