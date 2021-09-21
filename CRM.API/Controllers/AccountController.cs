using AutoMapper;
using CRM.API.Extensions;
using CRM.API.Models;
using CRM.Business.Models;
using CRM.Business.Services;
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
        public async Task<ActionResult<int>> AddAccountAsync([FromBody] AccountInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var dto = _mapper.Map<AccountDto>(inputModel);
            var accountId = await _accountService.AddAccountAsync(dto, leadInfo);
            return StatusCode(201, accountId);
        }

        // api/account/3
        [HttpDelete("account/{accountId}")]
        [Description("Delete lead account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> DeleteAccountAsync(int accountId)
        {
            var leadId = this.GetLeadId();
            await _accountService.DeleteAccountAsync(accountId, leadId);
            return NoContent();
        }

        // api/account/{accountId}
        [HttpPut("{accountId}")]
        [Description("Restore account")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> RestoreAccountAsync(int accountId)
        {
            var leadInfo = this.GetLeadInfo();
            /*var output =*/
            await _accountService.RestoreAccountAsync(accountId, leadInfo.LeadId);
            //return StatusCode(201, output);
            return NoContent();
        }

        // api/account/{accountId}
        [HttpGet("{accountId}")]
        [Description("Get account with transactions")]
        [ProducesResponseType(typeof(AccountBusinessModel), StatusCodes.Status200OK)]
        public async Task<AccountBusinessModel> GetAccountWithTransactionsAsync(int accountId)
        {
            var leadInfo = this.GetLeadInfo();
            return await _accountService.GetAccountWithTransactionsAsync(accountId, leadInfo);
        }

        // api/account/by-period
        [HttpPost("by-period")]
        [Description("Get transactions by period or period and account id")]
        [ProducesResponseType(typeof(List<AccountBusinessModel>), StatusCodes.Status200OK)]
        public async Task<List<AccountBusinessModel>> GetTransactionsByPeriodAndPossiblyAccountIdAsync([FromBody] TimeBasedAcquisitionInputModel model)
        {
            var leadInfo = this.GetLeadInfo();
            var dto = _mapper.Map<TimeBasedAcquisitionBusinessModel>(model);
            var output = await _accountService.GetTransactionsByPeriodAndPossiblyAccountIdAsync(dto, leadInfo);
            return _mapper.Map<List<AccountBusinessModel>>(output);
        }

        // api/account/by-accountIds
        [HttpPost("by-accountIds")]
        [Description("Get transactions by two months and account ids")]
        [ProducesResponseType(typeof(List<TransactionOutputModel>), StatusCodes.Status200OK)]
        public async Task<List<TransactionOutputModel>> GetTransactionsByTwoMonthAndAccountIdsAsync([FromBody] List<int> accounts)
        {
            var leadInfo = this.GetLeadInfo();
            var output = await _accountService.GetTransactionsByAccountIdsForTwoMonthsAsync(accounts, leadInfo);
            return _mapper.Map<List<TransactionOutputModel>>(output);
        }

        // api/account/lead/{leadId}
        [HttpGet("lead/{leadId}")]
        [Description("Get account with transactions")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<AccountBusinessModel>> GetLeadBalanceAsync(int leadId)
        {
            var leadInfo = this.GetLeadInfo();
            var output = await _accountService.GetLeadBalanceAsync(leadId, leadInfo);
            return StatusCode(200, output);
        }
    }
}