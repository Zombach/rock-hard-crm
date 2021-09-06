using AutoMapper;
using CRM.API.Models;
using CRM.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using CRM.API.Common;
using CRM.DAL.Enums;
using CRM.DAL.Models;

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

        // api/CommissionFee
        [HttpGet]
        [Description("Return all commission fees")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public List<CommissionFeeOutputModel> GetAllCommissionFees()
        {
            var listDto = _commissionFeeService.GetAllCommissionFees();
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }

        // api/CommissionFee
        [HttpGet("{accountId}")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public List<CommissionFeeOutputModel> GetCommissionFeesByAccountId(int accountId)
        {
            var listDto = _commissionFeeService.GetCommissionFeesByAccountId(accountId);
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }

        // api/CommissionFee
        [HttpGet("{leadId}")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public List<CommissionFeeOutputModel> GetCommissionFeesByLeadId(int leadId)
        {
            var listDto = _commissionFeeService.GetCommissionFeesByLeadId(leadId);
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }

        // api/CommissionFee/by-period
        [HttpPost("by-period")]
        [Description("Return all commission fees")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public List<CommissionFeeOutputModel> GetCommissionFeesByPeriod([FromBody] TimeBasedAcquisitionInputModel model)
        {
            var dto = _mapper.Map<TimeBasedAcquisitionDto>(model);
            var listDto = _commissionFeeService.GetCommissionFeesByPeriod(dto);
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }

        // api/CommissionFee
        [HttpGet("{role}")]
        [Description("Return all commission fees")]
        [ProducesResponseType(typeof(List<CommissionFeeOutputModel>), StatusCodes.Status200OK)]
        public List<CommissionFeeOutputModel> GetCommissionFeesByRole(Role role)
        {
            var listDto = _commissionFeeService.GetCommissionFeesByRole(role);
            return _mapper.Map<List<CommissionFeeOutputModel>>(listDto);
        }
    }
}