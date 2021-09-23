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
using System.Threading.Tasks;

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
        public async Task<LeadOutputModel> UpdateLeadByIdAsync([FromBody] LeadUpdateInputModel model)
        {
            var id = this.GetLeadId();
            var dto = _mapper.Map<LeadDto>(model);
            dto = await _leadService.UpdateLeadAsync(id, dto);
            return _mapper.Map<LeadOutputModel>(dto);
        }

        // api/lead/5/role/role
        [AuthorizeRoles(Role.Admin)]
        [HttpPut("{id}/role/{role}")]
        [Description("Update lead role")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        public async Task<LeadOutputModel> UpdateLeadRoleAsync(int id, Role role)
        {
            var dto = await _leadService.UpdateLeadRoleAsync(id, role);
            return _mapper.Map<LeadOutputModel>(dto);
        }

        // api/lead/by-filters
        [AuthorizeRoles(Role.Admin)]
        [HttpPost("by-filters")]
        [Description("Get all Leads by filters")]
        [ProducesResponseType(typeof(List<LeadByFiltersOutputModel>), StatusCodes.Status200OK)]
        public List<LeadByFiltersOutputModel> GetAllLeadsByFilters(
            [FromBody] LeadFiltersInputModel leadFilter)
        {
            var filter = _mapper.Map<LeadFiltersDto>(leadFilter);
            var leads = _leadService.GetLeadsByFilters(filter);
            var result = _mapper.Map<List<LeadByFiltersOutputModel>>(leads);
            return result;
        }

        // api/lead/by-batches/0
        [AuthorizeRoles(Role.Admin)]
        [HttpGet("by-batches/cursorId/{lastLeadId}")]
        [Description("Get all Leads by batches")]
        [ProducesResponseType(typeof(List<LeadByBatchesOutputModel>), StatusCodes.Status200OK)]
        public List<LeadByBatchesOutputModel> GetAllLeadsByBatсhes(int lastLeadId)
        {
            var leads = _leadService.GetAllLeadsByBatches(lastLeadId);
            var result = _mapper.Map<List<LeadByBatchesOutputModel>>(leads);
            return result;
        }

        // api/lead/change-role-leads
        [AuthorizeRoles(Role.Admin)]
        [HttpPut("change-role-leads")]
        [Description("Change role for leads list")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        public async Task<int> ChangeRoleForLeadsBulk([FromBody] List<LeadIdAndRoleInputModel> model)
        {
            var lead = this.GetLeadInfo();            
            var dto = _mapper.Map<List<LeadDto>>(model);
            await _leadService.UpdateLeadRoleBulkAsync(dto);
            return StatusCodes.Status200OK;
        }

        // api/lead
        [HttpDelete]
        [Description("Delete lead")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteLeadAsync()
        {
            var leadId = this.GetLeadId();
            await _leadService.DeleteLeadAsync(leadId);
            return NoContent();
        }

        // api/lead/3
        [HttpGet("{id}")]
        [Description("Get lead by id")]
        [ProducesResponseType(typeof(LeadOutputModel), StatusCodes.Status200OK)]
        public async Task<LeadOutputModel> GetLeadByIdAsync(int id)
        {
            var leadInfo = this.GetLeadInfo();
            var dto = await _leadService.GetLeadByIdAsync(id, leadInfo);
            var outPut = _mapper.Map<LeadOutputModel>(dto);
            return outPut;
        }

        // api/lead
        [AuthorizeRoles(Role.Admin)]
        [HttpGet]
        [Description("Get all Leads")]
        [ProducesResponseType(typeof(List<LeadOutputModel>), StatusCodes.Status200OK)]
        public async Task<List<LeadOutputModel>> GetAllLeadsAsync()
        {
            var listDto = await _leadService.GetAllLeadsAsync();
            var listOutPut = _mapper.Map<List<LeadOutputModel>>(listDto);
            return listOutPut;
        }
    }
}