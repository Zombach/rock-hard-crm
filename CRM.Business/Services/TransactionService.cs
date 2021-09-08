using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Core;
using DevEdu.Business.ValidationHelpers;
using Microsoft.Extensions.Options;
using RestSharp;
using static CRM.Business.TransactionEndpoint;

namespace CRM.Business.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IAccountValidationHelper _accountValidationHelper;

        public TransactionService
        (
            IOptions<ConnectionUrl> options,
            IAccountValidationHelper accountValidationHelper
        )
        {
            _client = new RestClient(options.Value.TstoreUrl);
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
        }

        public long AddDeposit(TransactionBusinessModel model)
        {
            var account = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(model.AccountId);
            model.AccountId = account.Id;
            model.Currency = account.Currency;

            var request = _requestHelper.CreatePostRequest(AddDepositEndpoint, model);
            var result = _client.Execute<long>(request);
            return result.Data;
        }

        public long AddWithdraw(TransactionBusinessModel model)
        {
            var account = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(model.AccountId);
            model.AccountId = account.Id;
            model.Currency = account.Currency;

            var request = _requestHelper.CreatePostRequest(AddWithdrawEndpoint, model);
            var result = _client.Execute<long>(request);
            return result.Data;
        }

        public string AddTransfer(TransferBusinessModel model)
        {
            var account = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(model.AccountId);
            var recipientAccount = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(model.RecipientAccountId);

            model.Currency = account.Currency;
            model.RecipientCurrency = recipientAccount.Currency;
            var request = _requestHelper.CreatePostRequest(AddTransferEndpoint, model);
            var result = _client.Execute<string>(request);
            return result.Data;
        }
    }
}