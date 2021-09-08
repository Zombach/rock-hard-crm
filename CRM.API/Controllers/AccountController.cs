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
            var leadId = this.GetLeadId();
            var dto = _mapper.Map<AccountDto>(inputModel);
            var accountId = _accountService.AddAccount(dto, leadId);
            return StatusCode(201, accountId);
        }

        // api/account/3
        [HttpDelete("account/{id}")]
        [Description("Delete lead account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult DeleteAccountById(int id)
        {
            var leadId = this.GetLeadId();
            _accountService.DeleteAccount(id, leadId);
            return NoContent();
        }

        // api/account/{accountId}
        [HttpGet("{accountId}")]
        [Description("Get account with transactions")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public ActionResult<List<TransactionBusinessModel>> GetAccountWithTransactions(int accountId)
        {
            var leadInfo = this.GetLeadIdAndRoles();
            var output = _accountService.GetAccountWithTransactions(accountId, leadInfo);
            return StatusCode(201, output);
        }
    }
}