using AutoMapper;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Business.ValidationHelpers;
using CRM.Core;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using static CRM.Business.Constants.TransactionEndpoint;

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
            IOptions<ConnectionSettings> options,
            IMapper mapper,
            IAccountValidationHelper accountValidationHelper
        )
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _mapper = mapper;
            _client = new RestClient(options.Value.TransactionStoreUrl);
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
        }

        public int AddAccount(AccountDto dto, LeadIdentityInfo leadInfo)
        {
            var lead = _leadRepository.GetLeadById(leadInfo.LeadId);
            _accountValidationHelper.CheckForDuplicateCurrencies(lead, dto.Currency);
            _accountValidationHelper.CheckForVipAccess(dto.Currency, leadInfo);
            dto.LeadId = lead.Id;
            var accountId = _accountRepository.AddAccount(dto);
            return accountId;
        }

        public void DeleteAccount(int accountId, int leadId)
        {
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadId);

            _accountRepository.DeleteAccount(accountId);
        }

        public void RestoreAccount(int accountId, int leadId)
        {
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadId);

            _accountRepository.RestoreAccount(accountId);
        }

        public AccountBusinessModel GetAccountWithTransactions(int accountId, LeadIdentityInfo leadInfo)
        {
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            if (!leadInfo.IsAdmin())
                _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);

            var accountModel = _mapper.Map<AccountBusinessModel>(dto);
            var request = _requestHelper.CreateGetRequest($"{GetTransactionsByAccountIdEndpoint}{accountId}");

            var response = _client.Execute<string>(request);

            accountModel.AddDeserializedTransactions(response.Data);

            accountModel.BalanceCalculation(accountId);

            return accountModel;
        }

        public List<AccountBusinessModel> GetTransactionsByPeriodAndPossiblyAccountId(TimeBasedAcquisitionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var list = new List<AccountBusinessModel>();
            if (model.AccountId != null)
            {
                var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound((int)model.AccountId);
                if (!leadInfo.IsAdmin())
                    _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);
                var accountModel = _mapper.Map<AccountBusinessModel>(dto);
                var request = _requestHelper.CreatePostRequest($"{GetTransactionsByPeriodEndpoint}", model);
                request.AddHeader("LeadId", leadInfo.LeadId.ToString());

                var response = _client.Execute<string>(request);
                accountModel.AddDeserializedTransactions(response.Data);


                accountModel.BalanceCalculation((int)model.AccountId);
                list.Add(accountModel);
            }

            else
            {
                if (!leadInfo.IsAdmin()) throw new AuthorizationException($"{ServiceMessages.NoAdminRights}");

                var request = _requestHelper.CreatePostRequest($"{GetTransactionsByPeriodEndpoint}", model);
                request.AddHeader("LeadId", leadInfo.LeadId.ToString());

                do
                {
                    var response = _client.Execute<string>(request);
                    list.AddDeserializedTransactions(response.Data);
                }
                while (AccountBusinessModelExtension.IsPart);

                GetAccountsInfoAndBalance(list);
            }

            return list;
        }

        private void GetAccountsInfoAndBalance(List<AccountBusinessModel> list)
        {
            foreach (var item in list)
            {
                var account = _accountRepository.GetAccountById(item.Id);
                item.LeadId = account.LeadId;
                item.Currency = account.Currency;
                item.CreatedOn = account.CreatedOn;
                item.IsDeleted = account.IsDeleted;
                item.Closed = account.Closed;
                item.BalanceCalculation(item.Id);
            }
        }

        public AccountBusinessModel GetLeadBalance(int leadId, LeadIdentityInfo leadInfo)
        {
            throw new NotImplementedException();
        }
    }
}