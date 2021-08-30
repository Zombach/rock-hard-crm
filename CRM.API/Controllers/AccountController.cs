using AutoMapper;
using CRM.API.Models;
using CRM.Business.Models;
using CRM.Business.Services;
using CRM.DAL.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using CRM.API.Extensions;
using Microsoft.AspNetCore.Authorization;

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
            var userInfo = this.GetUserIdAndRoles();
            var dto = _mapper.Map<AccountDto>(inputModel);
            var accountId = _accountService.AddAccount(dto, userInfo);
            return StatusCode(201, accountId);
        }

        // api/account/3
        [HttpDelete("account/{id}")]
        [Description("Delete lead account")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult DeleteAccountById(int id)
        {
            var userInfo = this.GetUserIdAndRoles();
            _accountService.DeleteAccount(id, userInfo);
            return NoContent();
        }

        // api/account/{accountId}
        [HttpGet("{accountId}")]
        [Description("Get account with transactions")]
        [ProducesResponseType(typeof(int), StatusCodes.Status201Created)]
        public ActionResult<List<TransactionBusinessModel>> GetAccountWithTransactions(int accountId)
        {
            var userInfo = this.GetUserIdAndRoles();
            var output = _accountService.GetAccountWithTransactions(accountId, userInfo);
            return StatusCode(201, output);
        }
    }
}