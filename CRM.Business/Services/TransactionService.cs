using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Business.Serialization;
using CRM.Business.ValidationHelpers;
using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CRM.Business.Constants.TransactionEndpoint;
using System.Runtime.Caching;
using MassTransit;
using CRM.DAL.Repositories;

namespace CRM.Business.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IAccountValidationHelper _accountValidationHelper;
        private readonly ILeadValidationHelper _leadValidationHelper;
        private readonly IAccountService _accountService;
        private readonly ICommissionFeeService _commissionFeeService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly decimal _commission;
        private readonly decimal _vipCommission;
        private readonly decimal _commissionModifier;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILeadRepository _leadRepository;
        
        private static ObjectCache _cacheModel = MemoryCache.Default;
        private static ObjectCache _cacheTransactionType = MemoryCache.Default;
        private string _transactionType;
        CacheItemPolicy _policy = new CacheItemPolicy();
        public TransactionService
        (
            IOptions<ConnectionSettings> connectionOptions,
            IOptions<CommissionSettings> commissionOptions,
            IAccountValidationHelper accountValidationHelper,
            ILeadValidationHelper leadValidationHelper,
            IAccountService accountService,
            IEmailSenderService emailSenderService,
            ICommissionFeeService commissionFeeService
        )
        {
            _client = new RestClient(connectionOptions.Value.TransactionStoreUrl);
            _client.AddHandler("application/json", () => NewtonsoftJsonSerializer.Default);
            _commission = commissionOptions.Value.Commission;
            _vipCommission = commissionOptions.Value.VipCommission;
            _commissionModifier = commissionOptions.Value.CommissionModifier;
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
            _leadValidationHelper = leadValidationHelper;
            _accountService = accountService;
            _emailSenderService = emailSenderService;
            _commissionFeeService = commissionFeeService;
        }

        public async Task<CommissionFeeDto> AddDepositAsync(TransactionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadInfo.LeadId);
            var account = await CheckAccessAndReturnAccount(model.AccountId, leadInfo);
            _accountValidationHelper.CheckForVipAccess(account.Currency, leadInfo);
            var commission = CalculateCommission(model.Amount, leadInfo);

            model.Amount -= commission;
            model.AccountId = account.Id;
            model.Currency = account.Currency;

            var request = _requestHelper.CreatePostRequest(AddDepositEndpoint, model);
            var result = _client.Execute<long>(request);
            if (!result.IsSuccessful)
            {
                throw new Exception($"{result.ErrorMessage} {_client.BaseUrl}");
            }
            var transactionId = result.Data;
            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.DepositSubject, string.Format(EmailMessages.DepositBody, model.Amount));
            var dto = new CommissionFeeDto
            { LeadId = leadDto.Id, AccountId = model.AccountId, TransactionId = transactionId, Role = leadDto.Role, CommissionAmount = commission, TransactionType = TransactionType.Deposit };

            dto.Id = await AddCommissionFee(dto);

            return dto;
        }

        public async Task<CommissionFeeDto> AddWithdrawAsync(TransactionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadInfo.LeadId);
            var accountModel = await _accountService.GetAccountWithTransactionsAsync(model.AccountId, leadInfo);
            _accountValidationHelper.CheckForVipAccess(accountModel.Currency, leadInfo);
            var commission = CalculateCommission(model.Amount, leadInfo);
            _accountValidationHelper.CheckBalance(accountModel, model.Amount);
            model.Date = _accountValidationHelper.GetTransactionsLastDateAndThrowIfNotFound(accountModel);

            model.Amount -= commission;
            model.AccountId = accountModel.Id;
            model.Currency = accountModel.Currency;

            var request = _requestHelper.CreatePostRequest(AddWithdrawEndpoint, model);
            var result = _client.Execute<long>(request);
            var transactionId = result.Data;

            _accountValidationHelper.CheckForDuplicateTransaction(transactionId, accountModel);

            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.WithdrawSubject, string.Format(EmailMessages.WithdrawBody, model.Amount));

            var dto = new CommissionFeeDto
            { LeadId = leadInfo.LeadId, AccountId = model.AccountId, TransactionId = transactionId, Role = leadInfo.Role, CommissionAmount = commission, TransactionType = TransactionType.Withdraw };

            dto.Id = await AddCommissionFee(dto);

            return dto;
        }

        public async Task<CommissionFeeDto> AddTransferAsync(TransferBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadInfo.LeadId);
            var accountModel = await _accountService.GetAccountWithTransactionsAsync(model.AccountId, leadInfo);
            var recipientAccount = await CheckAccessAndReturnAccount(model.RecipientAccountId, leadInfo);           
            var commission = CalculateCommission(model.Amount, leadInfo);
            _accountValidationHelper.CheckBalance(accountModel, model.Amount);
            model.Date = _accountValidationHelper.GetTransactionsLastDateAndThrowIfNotFound(accountModel);

            if (accountModel.Currency != Currency.RUB && accountModel.Currency != Currency.USD && !leadInfo.IsVip())
            {
                commission *= _commissionModifier;
                if (accountModel.Balance != model.Amount)
                {
                    throw new ValidationException(nameof(model.Amount), $"{ServiceMessages.IncompleteTransfer}");
                }
            }

            model.Amount -= commission;
            model.Currency = accountModel.Currency;
            model.RecipientCurrency = recipientAccount.Currency;


            var request = _requestHelper.CreatePostRequest(AddTransferEndpoint, model);
            var result = await _client.ExecuteAsync<List<long>>(request);

            if (result.Data == null)
            {
                throw new Exception("tstore slomalsy");
            }
            var transactionId = result.Data.First();
            _accountValidationHelper.CheckForDuplicateTransaction(transactionId, accountModel);
            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.TransferSubject, string.Format(EmailMessages.TransferBody, model.Amount, model.Currency, model.RecipientCurrency));

            var dto = new CommissionFeeDto
            { LeadId = leadInfo.LeadId, AccountId = model.AccountId, TransactionId = transactionId, Role = leadInfo.Role, CommissionAmount = commission, TransactionType = TransactionType.Transfer };

            dto.Id = await AddCommissionFee(dto);

            return dto;
        }


        public async Task CheckDepositTransactionAndSendEmailAsync(TransactionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadRepository.GetLeadByIdAsync(leadInfo.LeadId);
            var account = await _accountService.GetAccountWithTransactionsAsync(model.AccountId, leadInfo);
            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.TwoFactorAuthSubject, EmailMessages.TwoFactorAuthBody);
            _transactionType = AddDepositEndpoint;
            _cacheTransactionType.Set($"{leadInfo.LeadId}/", _transactionType, _policy);
            _cacheModel.Set($"{leadInfo.LeadId}", model, _policy);
        }

        public async Task CheckWithdrawTransactionAndSendEmailAsync(TransactionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadRepository.GetLeadByIdAsync(leadInfo.LeadId);
            var account = await _accountService.GetAccountWithTransactionsAsync(model.AccountId, leadInfo);
            _accountValidationHelper.CheckBalance(account, model.Amount);
            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.TwoFactorAuthSubject, EmailMessages.TwoFactorAuthBody);
            _transactionType = AddWithdrawEndpoint;
            _cacheTransactionType.Set($"{leadInfo.LeadId}/", _transactionType, _policy);
            _cacheModel.Set($"{leadInfo.LeadId}", model, _policy);
        }

        public async Task CheckTransferAndSendEmailAsync(TransferBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = await _leadRepository.GetLeadByIdAsync(leadInfo.LeadId);
            var recipientAccount = await CheckAccessAndReturnAccount(model.RecipientAccountId, leadInfo);
            var account = await _accountService.GetAccountWithTransactionsAsync(model.AccountId, leadInfo);
            _accountValidationHelper.CheckBalance(account, model.Amount);
            await _emailSenderService.EmailSenderAsync(leadDto, EmailMessages.TwoFactorAuthSubject, EmailMessages.TwoFactorAuthBody);
            _transactionType = AddTransferEndpoint;
            _cacheTransactionType.Set($"{leadInfo.LeadId}/", _transactionType, _policy);
            _cacheModel.Set($"{leadInfo.LeadId}", model, _policy);
        }

        public async Task<CommissionFeeDto> ContinueTransaction(LeadIdentityInfo leadInfo)
        {
            var transTypeContiue = (string)_cacheTransactionType[$"{leadInfo.LeadId}/"];
            if(transTypeContiue != AddTransferEndpoint)
            {
                if (transTypeContiue== AddDepositEndpoint)
                {
                    return await AddDepositAsync((TransactionBusinessModel)_cacheModel[$"{leadInfo.LeadId}"], leadInfo); 
                }
                else
                {
                    return await AddWithdrawAsync((TransactionBusinessModel)_cacheModel[$"{leadInfo.LeadId}"], leadInfo);
                }
            }
            return await AddTransferAsync((TransferBusinessModel)_cacheModel[$"{leadInfo.LeadId}"], leadInfo);
        }
        private async Task<AccountDto> CheckAccessAndReturnAccount(int accountId, LeadIdentityInfo leadInfo)
        {
            var account = await _accountValidationHelper.GetAccountByIdAndThrowIfNotFoundAsync(accountId);
            _accountValidationHelper.CheckLeadAccessToAccount(account.LeadId, leadInfo.LeadId);
            return account;
        }

        private async Task<int> AddCommissionFee(CommissionFeeDto dto)
        {
            return await _commissionFeeService.AddCommissionFeeAsync(dto);
        }

        private decimal CalculateCommission(decimal amount, LeadIdentityInfo leadInfo)
        {
            return leadInfo.IsVip() ? amount * _vipCommission : amount * _commission;
        }
    }
}