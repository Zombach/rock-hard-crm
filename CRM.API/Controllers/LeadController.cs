using AutoMapper;
using CRM.API.Models;
using CRM.Business.Services;
using CRM.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CRM.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeadController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILeadService _leadService;

        public LeadController(IMapper mapper, ILeadService leadService)
        {
            _mapper = mapper;
            _leadService = leadService;
        }

        // api/addLead
        [HttpPost("addLead")]
        [Description("Add new lead")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status201Created)]
        public ActionResult<LeadOutputModel> AddLead([FromBody] LeadInputModel model)
        {
            var dto = _mapper.Map<LeadDto>(model);
            var addedLead = _mapper.Map<LeadOutputModel>(_leadService.AddLead(dto));
            return StatusCode(201, addedLead);
        }

        // api/lead/userId
        [HttpPut("{leadId}")]
        [Description("Update lead")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        public LeadOutputModel UpdateUserById(int leadId, [FromBody] LeadUpdateInputModel model)
        {
            var dto = _mapper.Map<LeadDto>(model);
            dto = _leadService.UpdateLead(leadId, dto);
            return _mapper.Map<LeadOutputModel>(dto);
        }
    }
}