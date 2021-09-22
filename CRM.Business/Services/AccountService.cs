using AutoMapper;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using MailExchange;
using MassTransit;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static CRM.Business.Constants.TransactionEndpoint;

namespace CRM.Business.Services
{
    public partial class AccountService : IAccountService
    {
        private const string FinishResponse = "[]";
        private readonly IAccountRepository _accountRepository;
        private readonly ILeadRepository _leadRepository;
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IAccountValidationHelper _accountValidationHelper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AccountService
        (
            IAccountRepository accountRepository,
            ILeadRepository leadRepository,
            IMapper mapper,
            IAccountValidationHelper accountValidationHelper,
            IPublishEndpoint publishEndpoint,
            RestClient restClient
        )
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _mapper = mapper;
            _client = restClient;
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<int> AddAccountAsync(AccountDto accountDto, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadRepository.GetLeadByIdAsync(leadInfo.LeadId);
            _accountValidationHelper.CheckForDuplicateCurrencies(leadDto, accountDto.Currency);
            _accountValidationHelper.CheckForVipAccess(accountDto.Currency, leadInfo);

            accountDto.LeadId = leadDto.Id;
            var accountId = await _accountRepository.AddAccountAsync(accountDto);
            await EmailSenderAsync(leadDto, EmailMessages.AccountAddedSubject, EmailMessages.AccountAddedBody, accountDto);

            return accountId;
        }

        public async Task DeleteAccountAsync(int accountId, int leadId)
        {
            var leadDto = await _leadRepository.GetLeadByIdAsync(leadId);
            var accountDto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);

            _accountValidationHelper.CheckLeadAccessToAccount(accountDto.LeadId, leadId);
            await EmailSenderAsync(leadDto, EmailMessages.AccountDeleteSubject, EmailMessages.AccountDeleteBody, accountDto);
            await _accountRepository.DeleteAccountAsync(accountId);
        }

        public async Task RestoreAccountAsync(int accountId, int leadId)
        {
            var leadDto = await _leadRepository.GetLeadByIdAsync(leadId);
            var accountDto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);

            _accountValidationHelper.CheckLeadAccessToAccount(accountDto.LeadId, leadId);
            await EmailSenderAsync(leadDto, EmailMessages.AccountRestoreSubject, EmailMessages.AccountRestoreBody, accountDto);
            await _accountRepository.RestoreAccountAsync(accountId);
        }

        public async Task<AccountBusinessModel> GetAccountWithTransactionsAsync(int accountId, LeadIdentityInfo leadInfo)
        {
            CleanListModels();
            var dto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);
            if (!leadInfo.IsAdmin())
                _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);

            var accountModel = _mapper.Map<AccountBusinessModel>(dto);
            var request = _requestHelper.CreateGetRequest($"{GetTransactionsByAccountIdEndpoint}{accountId}");
            var response = _client.Execute<string>(request);

            var model = await Task.Run(async () => await AddDeserializedTransactionsAsync(accountModel, response.Data));

            return BalanceCalculation(model, accountId);
        }

        public async Task<List<AccountBusinessModel>> GetTransactionsByPeriodAndPossiblyAccountIdAsync(TimeBasedAcquisitionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            CleanListModels();
            List<AccountBusinessModel> models = new();
            if (model.AccountId != null)
            {
                var dto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync((int)model.AccountId);
                if (!leadInfo.IsAdmin())
                    _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);

                var accountModel = _mapper.Map<AccountBusinessModel>(dto);
                models.Add(accountModel);
            }
            else 
            {
                if (!leadInfo.IsAdmin()) throw new AuthorizationException($"{ServiceMessages.NoAdminRights}");
            }

            var request = _requestHelper.CreatePostRequest($"{GetTransactionsByPeriodEndpoint}", model);
            request.AddHeader("LeadId", leadInfo.LeadId.ToString());
            request.Timeout = 3000000;

            do
            {
                var response = await _client.ExecuteAsync<string>(request);
                models = await Task.Run(async () => await AddDeserializedTransactionsAsync(models, response.Data));
                if (response.Data == FinishResponse) break;
            }
            while (true);

            var qq = JsonConvert.SerializeObject(models);
            return models;
        }

        public async Task<AccountBusinessModel> GetLeadBalanceAsync(int leadId, LeadIdentityInfo leadInfo)
        {
            await Task.Run(() => throw new NotImplementedException());
            return null;
        }

        private async Task EmailSenderAsync(LeadDto dto, string subject, string body, AccountDto accountDto)
        {
            await _publishEndpoint.Publish<IMailExchangeModel>(new
            {
                Subject = subject,
                Body = $"{dto.LastName} {dto.FirstName} {body} {accountDto.Currency}",
                DisplayName = "Best CRM",
                MailAddresses = $"{dto.Email}"
            });
        }
    }
}