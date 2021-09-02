﻿using AutoMapper;
using CRM.API.Models;
using CRM.Business.Models;
using CRM.Business.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;

namespace CRM.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ITransactionService _transactionService;

        public TransactionController(IMapper mapper, ITransactionService transactionService)
        {
            _mapper = mapper;
            _transactionService = transactionService;
        }

        // api/transaction/deposit
        [HttpPost("deposit")]
        [Description("Add deposit")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        public ActionResult<long> AddDeposit([FromBody] TransactionInputModel inputModel)
        {
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            var output = _transactionService.AddDeposit(model);

            return StatusCode(201, output);
        }

        // api/transaction/withdraw
        [HttpPost("withdraw")]
        [Description("Add withdraw")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        public ActionResult<long> AddWithdraw([FromBody] TransactionInputModel inputModel)
        {
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            var output = _transactionService.AddWithdraw(model);

            return StatusCode(201, output);
        }

        // api/transaction/transfer
        [HttpPost("transfer")]
        [Description("Add transfer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public ActionResult<string> AddTransfer([FromBody] TransactionInputModel inputModel)
        {
            var model = _mapper.Map<TransferBusinessModel>(inputModel);
            var output = _transactionService.AddTransfer(model);

            return StatusCode(201, output);
        }
    }
}