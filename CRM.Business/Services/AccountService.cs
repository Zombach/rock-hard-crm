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

        public int AddAccount(AccountDto accountDto, LeadIdentityInfo leadInfo)
        {
            var leadDto = _leadRepository.GetLeadById(leadInfo.LeadId);
            _accountValidationHelper.CheckForDuplicateCurrencies(leadDto, accountDto.Currency);
            _accountValidationHelper.CheckForVipAccess(accountDto.Currency, leadInfo);
            accountDto.LeadId = leadDto.Id;
            var accountId = _accountRepository.AddAccount(accountDto);
            EmailSender(leadDto, EmailMessages.AccountAddedSubject, EmailMessages.AccountAddedBody, accountDto);
            return accountId;
        }

        public void DeleteAccount(int accountId, int leadId)
        {
            var leadDto = _leadRepository.GetLeadById(leadId);
            var accountDto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            _accountValidationHelper.CheckLeadAccessToAccount(accountDto.LeadId, leadId);
            EmailSender(leadDto, EmailMessages.AccountDeleteSubject, EmailMessages.AccountDeleteBody, accountDto);
            _accountRepository.DeleteAccount(accountId);
        }

        public void RestoreAccount(int accountId, int leadId)
        {
            var leadDto = _leadRepository.GetLeadById(leadId);
            var accountDto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            _accountValidationHelper.CheckLeadAccessToAccount(accountDto.LeadId, leadId);
            EmailSender(leadDto, EmailMessages.AccountRestoreSubject, EmailMessages.AccountRestoreBody, accountDto);
            _accountRepository.RestoreAccount(accountId);
        }

        public AccountBusinessModel GetAccountWithTransactions(int accountId, LeadIdentityInfo leadInfo)
        {
            CleanListModels();
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            if (!leadInfo.IsAdmin())
                _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);

            var accountModel = _mapper.Map<AccountBusinessModel>(dto);
            var request = _requestHelper.CreateGetRequest($"{GetTransactionsByAccountIdEndpoint}{accountId}");

            var response = _client.Execute<string>(request);

            var model = Task.Run(async () => await AddDeserializedTransactionsAsync(accountModel, response.Data)).Result;

            return BalanceCalculation(model, accountId);
        }

        public async Task<List<AccountBusinessModel>>  GetTransactionsByPeriodAndPossiblyAccountId(TimeBasedAcquisitionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            CleanListModels();
            List<AccountBusinessModel> models = new();
            if (model.AccountId != null)
            {
                var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound((int)model.AccountId);
                if (!leadInfo.IsAdmin())
                    _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);
                var accountModel = _mapper.Map<AccountBusinessModel>(dto); //Чет не в курил нах это?
                models.Add(accountModel); // с этим
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
                var response = _client.Execute<string>(request);
                await AddDeserializedTransactionsAsync(models, response.Data);
                //models = AddDeserializedTransactionsAsync(models, response.Data);
                if (response.Data == FinishResponse) break;
            }
            while (true);

            return models;
        }

        public AccountBusinessModel GetLeadBalance(int leadId, LeadIdentityInfo leadInfo)
        {
            throw new NotImplementedException();
        }

        private void EmailSender(LeadDto dto, string subject, string body, AccountDto accountDto)
        {
            _publishEndpoint.Publish<IMailExchangeModel>(new
            {
                Subject = subject,
                Body = $"{dto.LastName} {dto.FirstName} {body} {accountDto.Currency}",
                DisplayName = "Best CRM",
                MailAddresses = $"{dto.Email}"
            });
        }
    }
}