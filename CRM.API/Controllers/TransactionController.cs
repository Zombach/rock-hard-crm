using AutoMapper;
using CRM.API.Extensions;
using CRM.API.Models;
using CRM.Business.Models;
using CRM.Business.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using MailExchange;
using MassTransit;

namespace CRM.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;
        private readonly IPublishEndpoint _publishEndpoint;

        public TransactionController(IMapper mapper, ITransactionService transactionService, IPublishEndpoint publishEndpoint)
        {
            _mapper = mapper;
            _transactionService = transactionService;
            _publishEndpoint = publishEndpoint;
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
            _publishEndpoint.Publish<IMailExchangeModel>(new {MailTo = "kotafrakt@gmail.com", Subject = "Deposit", Body = $"вы положили {inputModel.Amount} на {inputModel.AccountId}"});
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
            _publishEndpoint.Publish<IMailExchangeModel>(new { MailTo = "kotafrakt@mail.ru", Subject = "Withdraw", Body = $"вы сняли {inputModel.Amount} с {inputModel.AccountId} комиссия составила {output.Amount}" });
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
            _publishEndpoint.Publish<IMailExchangeModel>(new { MailTo = "zhekul.90@gmail.com", Subject = "Transfer", Body = $"вы перевели {inputModel.Amount} с {inputModel.AccountId} на {inputModel.RecipientAccountId} комиссия составила {output.Amount}" });
            return StatusCode(201, output);
        }
    }
}