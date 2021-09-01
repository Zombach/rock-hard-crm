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

        public int AddAccount(AccountDto dto, int leadId)
        {
            var lead = _leadRepository.GetLeadById(leadId);
            _accountValidationHelper.CheckForDuplicateCurrencies(lead, dto.Currency);
            dto.LeadId = lead.Id;
            var accountId = _accountRepository.AddAccount(dto);
            return accountId;
        }

        public void DeleteAccount(int id, int leadId)
        {
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(id);
            _accountValidationHelper.CheckLeadAccessToAccount(dto.Id, leadId);

            _accountRepository.DeleteAccount(id);
        }

        public AccountBusinessModel GetAccountWithTransactions(int id, int leadId)
        {
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(id);
            _accountValidationHelper.CheckLeadAccessToAccount(dto.Id, leadId);

            var accountModel = _mapper.Map<AccountBusinessModel>(dto);
            var request = _requestHelper.CreateGetRequest($"{GetTransactionsByAccountIdEndpoint}{id}");

            var transactions = _client.Execute<List<TransactionBusinessModel>>(request).Data;
            if (transactions == null) return accountModel;

            accountModel.Transactions = transactions;
            foreach (var transaction in accountModel.Transactions)
            {
                accountModel.Balance += transaction.Amount;
            }
            return accountModel;
        }
    }
}