using System.Collections.Generic;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using RestSharp;
using static CRM.Business.TransactionEndpoint;

namespace CRM.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private const string BaseEndpoint = "https://localhost:44386/";
        private string _endPoint;
        
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;

        public AccountService(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _client = new RestClient(BaseEndpoint);
            _requestHelper = new RequestHelper();
        }

        public int AddAccount(AccountDto dto)
        {
            var accountId = _accountRepository.AddAccount(dto);
            return accountId;
        }

        public void DeleteAccount(int id)
        {
            _accountRepository.DeleteAccount(id);
        }

        public List<TransactionModel> GetTransactionsByAccountId(int id)
        {
            _endPoint = string.Format(GetTransactionsByAccountIdEndpoint, id);
            var request = _requestHelper.CreateGetRequest(string.Format(GetTransactionsByAccountIdEndpoint, id));
            var response = _client.Execute<List<TransactionModel>>(request);
            return response.Data;
        }

        public long AddDeposit(TransactionModel model)
        {
            var request = _requestHelper.CreatePostRequest(AddDepositEndpoint, model);
            var result = _client.Execute<long>(request);
            return result.Data;
        }
    }
}