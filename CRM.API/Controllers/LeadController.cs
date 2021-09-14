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

        // api/lead/5/role/role
        [AuthorizeRoles(Role.Admin)]
        [HttpPut("{id}/role/{role}")]
        [Description("Update lead role")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        public LeadOutputModel UpdateLeadRole(int id, Role role)
        {
            var dto = _leadService.UpdateLeadRole(id, role);
            return _mapper.Map<LeadOutputModel>(dto);
        }

        // api/lead/filter
        [AuthorizeRoles(Role.Admin)]
        [HttpPost("by-filters")]
        [Description("Get all Leads by filters")]
        [ProducesResponseType(typeof(List<LeadOutputModel>), StatusCodes.Status200OK)]
        public List<LeadOutputModel> GetAllLeadsByFilters(
            [FromBody] LeadFiltersInputModel leadFilter)
        {            
            var filter = _mapper.Map<LeadFiltersDto>(leadFilter);
            var leads = _leadService.GetLeadsByFilters(filter); 
            var result = _mapper.Map<List<LeadOutputModel>>(leads);
            return result;
        }

        // api/lead
        [HttpDelete]
        [Description("Delete lead")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteLead()
        {
            var leadId = this.GetLeadId();
            _leadService.DeleteLead(leadId);
            return NoContent();
        }

        // api/lead/3
        [HttpGet("{id}")]
        [Description("Get lead by id")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        public LeadOutputModel GetLeadById(int id)
        {
            var leadInfo = this.GetLeadIdAndRoles();
            var dto = _leadService.GetLeadById(id, leadInfo);
            var outPut = _mapper.Map<LeadOutputModel>(dto);
            return outPut;
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
    }
}