using AutoMapper;
using CRM.API.Common;
using CRM.API.Models;
using CRM.Business.Services;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CRM.API.Controllers
{
    [AuthorizeRoles(Role.Admin)]
    [ApiController]
    [Route("api/[controller]")]
    public class CommissionFeeController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ICommissionFeeService _commissionFeeService;

        public CommissionFeeController(IMapper mapper, ICommissionFeeService commissionFeeService)
        {
            _mapper = mapper;
            _commissionFeeService = commissionFeeService;
        }

        // api/CommissionFee/by-period
        [HttpPost("by-period")]
        [Description("Return all commission fees for the period with or without lead id and account id")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public async Task<List<CommissionFeeOutputModel>> SearchingCommissionFeesForThePeriodAsync([FromBody] TimeBasedAcquisitionSearchingInputModes model)
        {
            var dto = _mapper.Map<TimeBasedAcquisitionDto>(model);
            var listDto = await _commissionFeeService.SearchingCommissionFeesForThePeriodAsync(dto);
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }

        // api/CommissionFee
        [HttpGet]
        [Description("Return all commission fees")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public async Task<List<CommissionFeeOutputModel>> GetAllCommissionFeesAsync()
        {
            var listDto = await _commissionFeeService.GetAllCommissionFeesAsync();
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }

        // api/CommissionFee/by-account/1
        [HttpGet("by-account/{accountId}")]
        [Description("Return all commission fees by accountId")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public async Task<List<CommissionFeeOutputModel>> GetCommissionFeesByAccountIdAsync(int accountId)
        {
            var listDto = await _commissionFeeService.GetCommissionFeesByAccountIdAsync(accountId);
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }

        // api/CommissionFee/by-lead/1
        [HttpGet("by-lead/{leadId}")]
        [Description("Return all commission fees by leadId")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public async Task<List<CommissionFeeOutputModel>> GetCommissionFeesByLeadIdAsync(int leadId)
        {
            var listDto = await _commissionFeeService.GetCommissionFeesByLeadIdAsync(leadId);
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }

        // api/CommissionFee/by-role
        [HttpGet("by-role/{role}")]
        [Description("Return all commission fees by role")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public async Task<List<CommissionFeeOutputModel>> GetCommissionFeesByRoleAsync(Role role)
        {
            var listDto = await _commissionFeeService.GetCommissionFeesByRoleAsync(role);
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }
    }
}