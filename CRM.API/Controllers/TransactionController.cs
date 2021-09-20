using AutoMapper;
using CRM.API.Extensions;
using CRM.API.Models;
using CRM.Business.Models;
using CRM.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

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
        public ActionResult<CommissionFeeShortOutputModel> AddDeposit([FromBody] TransactionInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            var commissionModel = _transactionService.AddDeposit(model, leadInfo);
            var output = _mapper.Map<CommissionFeeShortOutputModel>(commissionModel);
            return StatusCode(201, output);
        }

        // api/transaction/withdraw
        [HttpPost("withdraw")]
        [Description("Add withdraw")]
        [ProducesResponseType(typeof(CommissionFeeShortOutputModel), StatusCodes.Status201Created)]
        public ActionResult<CommissionFeeShortOutputModel> AddWithdraw([FromBody] TransactionInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            var commissionModel = _transactionService.AddWithdraw(model, leadInfo);
            var output = _mapper.Map<CommissionFeeShortOutputModel>(commissionModel);
            return StatusCode(201, output);
        }

        // api/transaction/transfer
        [HttpPost("transfer")]
        [Description("Add transfer")]
        [ProducesResponseType(typeof(CommissionFeeShortOutputModel), StatusCodes.Status201Created)]
        public ActionResult<CommissionFeeShortOutputModel> AddTransfer([FromBody] TransferInputModel inputModel)
        {
            var leadInfo = this.GetLeadInfo();
            var model = _mapper.Map<TransferBusinessModel>(inputModel);
            var commissionModel = _transactionService.AddTransfer(model, leadInfo);
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