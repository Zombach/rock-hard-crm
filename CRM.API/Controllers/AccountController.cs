using AutoMapper;
using CRM.API.Extensions;
using CRM.API.Models;
using CRM.API.Models.OutputModels;
using CRM.Business.Models;
using CRM.Business.Services;
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
    public class AccountController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IAccountService _accountService;

        public AccountController(IMapper mapper, IAccountService accountService)
        {
            _mapper = mapper;
            _accountService = accountService;
        }

        // api/account/create
        [HttpPost("account/create")]
        [Description("Create lead account")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public ActionResult<int> AddAccount([FromBody] AccountInputModel inputModel)
        {
            var leadInfo = this.GetLeadIdAndRoles();
            var dto = _mapper.Map<AccountDto>(inputModel);
            var accountId = _accountService.AddAccount(dto, leadInfo);
            return StatusCode(201, accountId);
        }

        // api/account/3
        [HttpDelete("account/{accountId}")]
        [Description("Delete lead account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult DeleteAccount(int accountId)
        {
            var leadId = this.GetLeadId();
            _accountService.DeleteAccount(accountId, leadId);
            return NoContent();
        }

        // api/account/{accountId}
        [HttpPut("{accountId}")]
        [Description("Restore account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public ActionResult RestoreAccount(int accountId)
        {
            var leadInfo = this.GetLeadIdAndRoles();
            /*var output =*/
            _accountService.RestoreAccount(accountId, leadInfo.LeadId);
            //return StatusCode(201, output);
            return NoContent();
        }

        // api/account/{accountId}
        [HttpGet("{accountId}")]
        [Description("Get account with transactions")]
        [ProducesResponseType(typeof(AccountBusinessModel), StatusCodes.Status200OK)]
        public AccountBusinessModel GetAccountWithTransactions(int accountId)
        {
            var leadInfo = this.GetLeadIdAndRoles();
            return _accountService.GetAccountWithTransactions(accountId, leadInfo);
        }

        // api/account/by-period
        [HttpPost("by-period")]
        [Description("Get transactions by period or period and account id")]
        [ProducesResponseType(typeof(List<TransactionOutputModel>), StatusCodes.Status200OK)]
        public List<AccountBusinessModel> GetTransactionsByPeriodAndPossiblyAccountId([FromBody] TimeBasedAcquisitionInputModel model)
        {
            var leadInfo = this.GetLeadIdAndRoles();
            var dto = _mapper.Map<TimeBasedAcquisitionBusinessModel>(model);
            var output = _accountService.GetTransactionsByPeriodAndPossiblyAccountId(dto, leadInfo);
            return _mapper.Map<List<AccountBusinessModel>>(output);
        }

        // api/account/lead/{leadId}
        [HttpGet("lead/{leadId}")]
        [Description("Get account with transactions")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public ActionResult<AccountBusinessModel> GetLeadBalance(int leadId)
        {
            var leadInfo = this.GetLeadIdAndRoles();
            var output = _accountService.GetLeadBalance(leadId, leadInfo);
            return StatusCode(200, output);
        }
    }
}