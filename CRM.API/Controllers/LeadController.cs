using AutoMapper;
using CRM.API.Common;
using CRM.API.Extensions;
using CRM.API.Models;
using CRM.Business.Services;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;

namespace CRM.API.Controllers
{
    [Authorize]
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

        // api/lead
        [HttpPut]
        [Description("Update lead")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        public LeadOutputModel UpdateLeadById([FromBody] LeadUpdateInputModel model)
        {
            var id = this.GetLeadId();
            var dto = _mapper.Map<LeadDto>(model);
            dto = _leadService.UpdateLead(id, dto);
            return _mapper.Map<LeadOutputModel>(dto);
        }

        // api/lead
        [AuthorizeRoles(Role.Admin)]
        [HttpGet]
        [Description("Get all Leads")]
        [ProducesResponseType(typeof(List<LeadOutputModel>), StatusCodes.Status200OK)]
        public List<LeadOutputModel> GetAllLeads()
        {
            var listDto = _leadService.GetAllLeads();
            var listOutPut = _mapper.Map<List<LeadOutputModel>>(listDto);
            return listOutPut;
        }

        // api/lead/3
        [HttpGet("{id}")]
        [Description("Return lead by id")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        public LeadOutputModel GetLeadById(int id)
        {
            var dto = _leadService.GetLeadById(id);
            var outPut = _mapper.Map<LeadOutputModel>(dto);
            return outPut;
        }

        // api/lead
        [HttpDelete]
        [Description("Delete lead by id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteLeadById()
        {
            var id = this.GetLeadId();
            _leadService.DeleteLeadById(id);
            return NoContent();
        }
    }
}