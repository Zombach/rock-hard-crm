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

        public TransactionController
            (
            IMapper mapper,
            ITransactionService transactionService,
            ITwoFactorAuthenticatorService twoFactorAuthService
            )
        {
            _mapper = mapper;
            _transactionService = transactionService;
            _twoFactorAuthService = twoFactorAuthService;
        }

        // api/transaction/deposit
        [HttpPost("deposit")]
        [Description("Add deposit")]
        [ProducesResponseType(typeof(CommissionFeeShortOutputModel), StatusCodes.Status201Created)]
        public async Task<ActionResult<CommissionFeeShortOutputModel>> AddDepositAsync([FromBody] TransactionInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            var commissionModel = await _transactionService.AddDepositAsync(model, leadInfo);
            var output = _mapper.Map<CommissionFeeShortOutputModel>(commissionModel);
            return StatusCode(201, output);
        }

        // api/transaction/withdraw
        [HttpPost("withdraw")]
        [Description("Add withdraw")]
        [ProducesResponseType(typeof(CommissionFeeShortOutputModel), StatusCodes.Status201Created)]
        public async Task<ActionResult<CommissionFeeShortOutputModel>> AddWithdrawAsync([FromBody] TransactionInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            var commissionModel = await _transactionService.AddWithdrawAsync(model, leadInfo);
            var output = _mapper.Map<CommissionFeeShortOutputModel>(commissionModel);
            return StatusCode(201, output);
        }

        // api/transaction/transfer
        [HttpPost("transfer")]
        [Description("Add transfer")]
        [ProducesResponseType(typeof(CommissionFeeShortOutputModel), StatusCodes.Status201Created)]
        public async Task<ActionResult<CommissionFeeShortOutputModel>> AddTransferAsync([FromBody] TransferInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransferBusinessModel>(inputModel);
            var commissionModel = await _transactionService.AddTransferAsync(model, leadInfo);
            var output = _mapper.Map<CommissionFeeShortOutputModel>(commissionModel);
            return StatusCode(201, output);
        }

        [HttpPost("two-factor-authentication")]
        [ProducesResponseType(typeof(CommissionFeeShortOutputModel), StatusCodes.Status201Created)]
        public ActionResult TwoFactorAuthentication(TwoFactorAuthenticatorModel tfaModel, string pinCode)
        {
            var leadInfo = this.GetLeadInfo();
            var isValid = _twoFactorAuthService.ValidateTwoFactorPIN(leadInfo.KeyForTwoFactorAuth, pinCode);
            if (!isValid)
            {
                return Redirect("two-factor-authentication");
            }

            return StatusCode(201, output);
        }
    }
}