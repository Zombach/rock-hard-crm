using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Core;
using CRM.DAL.Repositories;
using Microsoft.Extensions.Options;
using RestSharp;
using static CRM.Business.TransactionEndpoint;

namespace CRM.Business.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IAccountRepository _accountRepository;

        public TransactionService(IOptions<ConnectionUrl> options, IAccountRepository accountRepository)
        {
            _client = new RestClient(options.Value.TstoreUrl);
            _requestHelper = new RequestHelper();
            _accountRepository = accountRepository;
        }

        public long AddDeposit(int accountId, TransactionBusinessModel model)
        {
            model.AccountId = accountId;
            model.Currency = _accountRepository.GetAccountById(accountId).Currency;
            var request = _requestHelper.CreatePostRequest(AddDepositEndpoint, model);
            var result = _client.Execute<long>(request);
            return result.Data;
        }

        public long AddWithdraw(int accountId, TransactionBusinessModel model)
        {
            model.AccountId = accountId;
            model.Currency = _accountRepository.GetAccountById(accountId).Currency;
            var request = _requestHelper.CreatePostRequest(AddWithdrawEndpoint, model);
            var result = _client.Execute<long>(request);
            return result.Data;
        }

        public string AddTransfer(TransferBusinessModel model)
        {
            model.Currency = _accountRepository.GetAccountById(model.AccountId).Currency;
            model.RecipientCurrency = _accountRepository.GetAccountById(model.RecipientAccountId).Currency;
            var request = _requestHelper.CreatePostRequest(AddTransferEndpoint, model);
            var result = _client.Execute<string>(request);
            return result.Data;
        }
    }
}