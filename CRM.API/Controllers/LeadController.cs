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
        private readonly IAccountService _accountService;

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

        // api/lead/account
        [HttpPost("account")]
        [Description("Create lead account")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public ActionResult<int> AddAccount([FromBody] AccountInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var dto = _mapper.Map<AccountDto>(inputModel);
            var accountId = _accountService.AddAccount(dto, leadInfo);
            return StatusCode(201, accountId);
        }

        // api/lead/account/3
        [HttpDelete("account/{id}")]
        [Description("Delete lead account by id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult DeleteAccountById(int id)
        {
            var leadId = this.GetLeadId();
            _accountService.DeleteAccount(id, leadId);
            return NoContent();
        }
    }
}