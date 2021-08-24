using AutoMapper;
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

        // api/transaction/deposit/5
        [HttpPost("deposit/{accountId}")]
        [Description("Add deposit")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        public ActionResult<long> AddDeposit(int accountId, [FromBody] TransactionInputModel inputModel)
        {
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            var output = _transactionService.AddDeposit(accountId, model);

            return StatusCode(201, output);
        }

        // api/transaction/withdraw/5
        [HttpPost("withdraw/{accountId}")]
        [Description("Add withdraw")]
        [ProducesResponseType(typeof(long), StatusCodes.Status201Created)]
        public ActionResult<long> AddWithdraw(int accountId, [FromBody] TransactionInputModel inputModel)
        {
            var model = _mapper.Map<TransactionBusinessModel>(inputModel);
            var output = _transactionService.AddWithdraw(accountId, model);

            return StatusCode(201, output);
        }

        // api/transaction/transfer/from/{accountId}/to/{accountId}
        [HttpPost("transfer/from/{accountId}/to/{recipientId}")]
        [Description("Add transfer")]
        [ProducesResponseType(typeof(string), StatusCodes.Status201Created)]
        public ActionResult<string> AddTransfer(int accountId, int recipientId, [FromBody] TransactionInputModel inputModel)
        {
            var model = _mapper.Map<TransferBusinessModel>(inputModel);
            var output = _transactionService.AddTransfer(accountId, recipientId, model);

            return StatusCode(201, output);
        }
    }
}