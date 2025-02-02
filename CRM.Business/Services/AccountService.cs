﻿using AutoMapper;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using static CRM.Business.Constants.TransactionEndpoint;

namespace CRM.Business.Services
{
    public partial class AccountService : IAccountService
    {
        private const string FinishResponse = "[]";
        private readonly IEmailSenderService _emailSenderService;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IAccountValidationHelper _accountValidationHelper;
        private readonly ILeadValidationHelper _leadValidationHelper;

        public AccountService
        (
            IEmailSenderService emailSenderService,
            IAccountRepository accountRepository,
            IMapper mapper,
            IAccountValidationHelper accountValidationHelper,
            RestClient restClient,
            ILeadValidationHelper leadValidationHelper
        )
        {
            _emailSenderService = emailSenderService;
            _accountRepository = accountRepository;
            _mapper = mapper;
            _client = restClient;
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
            _leadValidationHelper = leadValidationHelper;
        }

        public async Task<int> AddAccountAsync(Currency currency, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadInfo.LeadId);
            _accountValidationHelper.CheckForDuplicateCurrencies(leadDto, currency);
            _accountValidationHelper.CheckForVipAccess(currency, leadInfo);

            var accountDto = new AccountDto { LeadId = leadInfo.LeadId, Currency = currency };
            var accountId = await _accountRepository.AddAccountAsync(accountDto);
            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.AccountAddedSubject, string.Format(EmailMessages.AccountRestoreBody, accountDto));
            return accountId;
        }

        public async Task DeleteAccountAsync(int accountId, int leadId)
        {
            var leadDto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
            var accountDto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);

            _accountValidationHelper.CheckLeadAccessToAccount(accountDto.LeadId, leadId);
            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.AccountDeleteSubject, string.Format(EmailMessages.AccountRestoreBody, accountDto));
            await _accountRepository.DeleteAccountAsync(accountId);
        }

        public async Task RestoreAccountAsync(int accountId, int leadId)
        {
            var leadDto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
            var accountDto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);

            _accountValidationHelper.CheckLeadAccessToAccount(accountDto.LeadId, leadId);
            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.AccountRestoreSubject, string.Format(EmailMessages.AccountRestoreBody, accountDto));
            await _accountRepository.RestoreAccountAsync(accountId);
        }

        public async Task<AccountBusinessModel> GetAccountWithTransactionsAsync(int accountId, LeadIdentityInfo leadInfo)
        {
            var leadId = leadInfo.LeadId.ToString();
            CleanListModels(leadId);
            var accountDto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);
            if (!leadInfo.IsAdmin())
                _accountValidationHelper.CheckLeadAccessToAccount(accountDto.LeadId, leadInfo.LeadId);

            var accountModel = _mapper.Map<AccountBusinessModel>(accountDto);
            var request = _requestHelper.CreateGetRequest($"{GetTransactionsByAccountIdEndpoint}{accountId}");
            var response = _client.Execute<string>(request);

            var accountBusinessModel = await Task.Run(async () => await AddDeserializedTransactionsAsync(accountModel, response.Data, leadId));

            CleanListModels(leadId);
            return BalanceCalculation(accountBusinessModel, accountId);
        }

        public async Task<List<AccountBusinessModel>> GetTransactionsByPeriodAndPossiblyAccountIdAsync(TimeBasedAcquisitionBusinessModel timeBasedModel, LeadIdentityInfo leadInfo)
        {
            var leadId = leadInfo.LeadId.ToString();
            CleanListModels(leadId);
            List<AccountBusinessModel> models = new();
            if (timeBasedModel.AccountId != null)
            {
                var dto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync((int)timeBasedModel.AccountId);
                if (!leadInfo.IsAdmin())
                    _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);

                var accountModel = _mapper.Map<AccountBusinessModel>(dto);
                models.Add(accountModel);
            }
            else
            {
                if (!leadInfo.IsAdmin()) throw new AuthorizationException($"{ServiceMessages.NoAdminRights}");
            }

            var request = _requestHelper.CreatePostRequest($"{GetTransactionsByPeriodEndpoint}", timeBasedModel);
            request.AddHeader("LeadId", leadId);
            request.Timeout = 3000000;

            do
            {
                var response = await _client.ExecuteAsync<string>(request);
                models = await Task.Run(async () => await AddDeserializedTransactionsAsync(models, response.Data, leadId));
                if (response.Data == FinishResponse) break;
            }
            while (true);

            CleanListModels(leadId);
            return models;
        }

        public async Task<List<TransactionBusinessModel>> GetTransactionsByAccountIdsForTwoMonthsAsync(List<int> accountIds, LeadIdentityInfo leadInfo)
        {
            foreach (var accountId in accountIds)
            {
                var dto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);
                if (!leadInfo.IsAdmin())
                    _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);
            }

            var request = _requestHelper.CreatePostRequest($"{GetTransactionsByAccountIdsForTwoMonthsEndpoint}", accountIds);
            request.Timeout = 3000000;
            var response = await _client.ExecuteAsync<string>(request);
            if (response.StatusCode != HttpStatusCode.OK) throw new Exception($"{response.ErrorMessage}");

            return System.Text.Json.JsonSerializer.Deserialize<List<TransactionBusinessModel>>(response.Data);
        }

        public async Task<decimal> GetLeadBalanceAsync(int leadId, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
            if (!leadInfo.IsAdmin())
                _leadValidationHelper.CheckAccessToLead(leadId, leadInfo);

            var request = _requestHelper.CreateGetRequest(GetCurrentCurrenciesRatesEndpoint);
            var rates = _client.Execute<RatesExchangeBusinessModel>(request).Data;

            return leadDto.Accounts
                .Select(account => GetAccountWithTransactionsAsync(account.Id, leadInfo).Result)
                .Select(accountBusinessModel => _accountValidationHelper.ConvertToRubble(accountBusinessModel, rates))
                .Sum();
        }
    }
}