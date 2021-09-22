using AutoMapper;
using CRM.API.Extensions;
using CRM.API.Models;
using CRM.Business.Models;
using CRM.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CRM.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;
        private readonly ILeadService _leadService;

        public TransactionController
            (
            IMapper mapper,
            ITransactionService transactionService,
            ITwoFactorAuthenticatorService twoFactorAuthService,
            ILeadService leadService
            )
        {
            _mapper = mapper;
            _transactionService = transactionService;
            _twoFactorAuthService = twoFactorAuthService;
            _leadService = leadService;
        }

        // api/transaction/deposit
        [HttpPost("deposit")]
        [Description("Add deposit")]
        [ProducesResponseType(typeof(CommissionFeeShortOutputModel), StatusCodes.Status201Created)]
        public async Task AddDepositAsync([FromBody] TransactionInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            await _transactionService.CheckTransactionAndSendEmailAsync(model, leadInfo);
        }

        // api/transaction/withdraw
        [HttpPost("withdraw")]
        [Description("Add withdraw")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task AddWithdrawAsync([FromBody] TransactionInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            await _transactionService.CheckTransactionAndSendEmailAsync(model, leadInfo);
        }

        // api/transaction/transfer
        [HttpPost("transfer")]
        [Description("Add transfer")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task AddTransferAsync([FromBody] TransferInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransferBusinessModel>(inputModel);
            await _transactionService.CheckTransferAndSendEmailAsync(model, leadInfo);
        }

        // api/transaction/two-factor-authentication/{pinCode}
        [HttpPost("two-factor-authentication/{pinCode}")]
        [Description("Two factor authentication")]
        [ProducesResponseType(typeof(CommissionFeeShortOutputModel), StatusCodes.Status201Created)]
        public async Task<ActionResult<CommissionFeeShortOutputModel>> TwoFactorAuthentication(string pinCode)
        {
            var leadInfo = this.GetLeadInfo();
            var key = await _leadService.GetTwoFactorKeyAsync(leadInfo.LeadId);
            var isValid = _twoFactorAuthService.ValidateTwoFactorPIN(key, pinCode);
            if (isValid)
            {
                var commissionModel = await _transactionService.ContinueTransaction(leadInfo);
                var output = _mapper.Map<CommissionFeeShortOutputModel>(commissionModel);
                return StatusCode(201, output);
            }
            return Unauthorized();
        }
    }
}