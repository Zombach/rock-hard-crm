using AutoMapper;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Core;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using DevEdu.Business.ValidationHelpers;
using Microsoft.Extensions.Options;
using RestSharp;
using System.Collections.Generic;
using Newtonsoft.Json;
using static CRM.Business.TransactionEndpoint;

namespace CRM.Business.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IAccountValidationHelper _accountValidationHelper;

        public AccountService
        (
            IAccountRepository accountRepository,
            ILeadRepository leadRepository,
            IMapper mapper,
            IAccountValidationHelper accountValidationHelper,
            RestClient client,
            RequestHelper requestHelper)
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _mapper = mapper;
            _accountValidationHelper = accountValidationHelper;
            _client = client;
            _requestHelper = requestHelper;
        }

        public AccountService
        (
            IAccountRepository accountRepository,
            ILeadRepository leadRepository,
            IOptions<ConnectionUrl> options,
            IMapper mapper,
            IAccountValidationHelper accountValidationHelper
        )
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _mapper = mapper;
            _client = new RestClient(options.Value.TstoreUrl);
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
        }

        public int AddAccount(AccountDto account, int leadId)
        {
            var lead = _leadRepository.GetLeadById(leadId);
            _accountValidationHelper.CheckForDuplicateCurrencies(lead, account.Currency);
            account.LeadId = lead.Id;
            var accountId = _accountRepository.AddAccount(account);
            return accountId;
        }

        public void DeleteAccount(int id, int leadId)
        {
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(id);
            _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadId);

            _accountRepository.DeleteAccount(id);
        }

        public AccountBusinessModel GetAccountWithTransactions(int id, int leadId)
        {
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(id);
            _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadId);

            var accountModel = _mapper.Map<AccountBusinessModel>(dto);
            var request = _requestHelper.CreateGetRequest($"{GetTransactionsByAccountIdEndpoint}{id}");

            var response = _client.Execute<string>(request);
            var transfers = new List<TransferBusinessModel>();
            var transactions = new List<TransactionBusinessModel>();
            var result = JsonConvert.DeserializeObject<List<TransferBusinessModel>>(response.Data);

            if (result != null)
                foreach (var obj in result)
                {
                    if (obj.RecipientAccountId != default)
                    {
                        transfers.Add(obj);
                    }
                    else
                    {
                        transactions.Add(obj);
                    }
                    accountModel.Balance += obj.Amount;
                }

            accountModel.Transactions = transactions;
            accountModel.Transfers = transfers;

            return accountModel;
        }
    }
}