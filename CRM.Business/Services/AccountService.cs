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
using MailExchange;
using MassTransit;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.DAL.Enums;
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
            IOptions<ConnectionSettings> options,
            IMapper mapper,
            IAccountValidationHelper accountValidationHelper,
            IPublishEndpoint publishEndpoint
        )
        {
            _accountRepository = accountRepository;
            _leadRepository = leadRepository;
            _mapper = mapper;
            _client = new RestClient(options.Value.TransactionStoreUrl);
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<int> AddAccountAsync(Currency currency, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadRepository.GetLeadByIdAsync(leadInfo.LeadId);
            _accountValidationHelper.CheckForDuplicateCurrencies(leadDto, currency);
            _accountValidationHelper.CheckForVipAccess(currency, leadInfo);
            var accountDto = new AccountDto {LeadId = leadInfo.LeadId, Currency = currency};
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
            var leadId = leadInfo.LeadId.ToString();
            CleanListModels(leadId);
            var dto = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);
            if (!leadInfo.IsAdmin())
                _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);

            var accountModel = _mapper.Map<AccountBusinessModel>(dto);
            var request = _requestHelper.CreateGetRequest($"{GetTransactionsByAccountIdEndpoint}{accountId}");

            var response = _client.Execute<string>(request);

            var model = await Task.Run(async () => await AddDeserializedTransactionsAsync(accountModel, response.Data, leadId));

            CleanListModels(leadId);
            return BalanceCalculation(model, accountId);
        }

        public async Task<List<AccountBusinessModel>> GetTransactionsByPeriodAndPossiblyAccountIdAsync(TimeBasedAcquisitionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            string leadId = leadInfo.LeadId.ToString();
            CleanListModels(leadId);
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