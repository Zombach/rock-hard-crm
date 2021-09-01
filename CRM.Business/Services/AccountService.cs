using System;
using AutoMapper;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Core;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Collections.Generic;
using CRM.Business.IdentityInfo;
using static CRM.Business.TransactionEndpoint;

namespace CRM.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;

        public AccountService(IAccountRepository accountRepository, RestClient client, IMapper mapper, RequestHelper requestHelper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _client = client;
            _requestHelper = requestHelper;
        }

        public AccountService(IAccountRepository accountRepository, IOptions<ConnectionUrl> options, IMapper mapper)
        {
            _accountRepository = accountRepository;
            _mapper = mapper;
            _client = new RestClient(options.Value.TstoreUrl);
            _requestHelper = new RequestHelper();
        }

        public int AddAccount(AccountDto dto, UserIdentityInfo userIdentityInfo)
        {
            dto.LeadId = userIdentityInfo.LeadId;
            var accountId = _accountRepository.AddAccount(dto);
            return accountId;
        }

        public void DeleteAccount(int id, UserIdentityInfo userIdentityInfo)
        {
            var dto = _accountRepository.GetAccountById(id);
            if (!userIdentityInfo.IsAdmin())
            {
                if (dto.LeadId!=userIdentityInfo.LeadId)
                {
                    throw new Exception("It is not your account");
                }
            }
            _accountRepository.DeleteAccount(id);
        }

        public AccountBusinessModel GetAccountWithTransactions(int id, UserIdentityInfo userIdentityInfo)
        {
            var accountDto = _accountRepository.GetAccountById(id);
            var accountModel = _mapper.Map<AccountBusinessModel>(accountDto);
            var request = _requestHelper.CreateGetRequest(string.Format(GetTransactionsByAccountIdEndpoint, id));
            var transactions = _client.Execute<List<TransactionBusinessModel>>(request).Data;

            accountModel.Transactions = transactions;
            foreach (var transaction in accountModel.Transactions)
            {
                accountModel.Balance += transaction.Amount;
            }
            return accountModel;
        }
    }
}