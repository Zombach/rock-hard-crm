using System.Collections.Generic;
using AutoMapper;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Core;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using Microsoft.Extensions.Options;
using RestSharp;
using static CRM.Business.TransactionEndpoint;

namespace CRM.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;

        public AccountService(IAccountRepository accountRepository, IOptions<ConnectionUrl> options, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _client = new RestClient(options.Value.TstoreUrl);
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

        public AccountBusinessModel GetTransactionsByAccountId(int id)
        {
            var accountDto = _accountRepository.GetAccountById(id);
            var accountModel = _mapper.Map<AccountBusinessModel>(accountDto);
            var request = _requestHelper.CreateGetRequest(string.Format(GetTransactionsByAccountIdEndpoint, id));
            accountModel.Transactions = _client.Execute<List<TransactionBusinessModel>>(request).Data;

            foreach(var transactions in accountModel.Transactions)
            {
                accountModel.Balance += transactions.Amount;
            }
            return accountModel;
        }
    }
}