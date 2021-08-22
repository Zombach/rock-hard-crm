using System.Collections.Generic;
using CRM.Business.Models;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using DevEdu.Core.Requests;
using RestSharp;

namespace CRM.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private const string BaseEndpoint = "https://localhost:44386/";
        private string _endPoint;
        private string GetTransactionsByAccountIdEndpoint;
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
            var request = _requestHelper.CreateGetRequest(_endPoint);
            var response = _client.Execute<List<TransactionModel>>(request);
            return response.Data;
        }
    }
}