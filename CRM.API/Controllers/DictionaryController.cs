using AutoMapper;
using CRM.API.Models;
using CRM.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DictionaryController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICityService _citySerivice;

        public DictionaryController(IMapper mapper, ICityService citySerivice)
        {
            _mapper = mapper;
            _citySerivice = citySerivice;
        }

        // api/dictionary/city
        [HttpGet("city")]
        [Description("Return list cities")]
        [ProducesResponseType(typeof(List<CityOutputModel>), StatusCodes.Status200OK)]
        public List<CityOutputModel> GetAllCities()
        {
            var listDto = _citySerivice.GetAllCities();
            var listOutPut = _mapper.Map<List<CityOutputModel>>(listDto);
            return listOutPut;
        }



    }
}
