using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Business.ValidationHelpers;
using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using CRM.DAL.Repositories;
using MailExchange;
using MassTransit;
using static CRM.Business.Constants.TransactionEndpoint;

namespace CRM.Business.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly RestClient _client;
        private readonly RequestHelper _requestHelper;
        private readonly IAccountValidationHelper _accountValidationHelper;
        private readonly IAccountService _accountService;
        private readonly ICommissionFeeService _commissionFeeService;
        private readonly decimal _commission;
        private readonly decimal _vipCommission;
        private readonly decimal _commissionModifier;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILeadRepository _leadRepository;

        public TransactionService
        (
            IOptions<ConnectionSettings> connectionOptions,
            IOptions<CommissionSettings> commissionOptions,
            IAccountValidationHelper accountValidationHelper,
            IAccountService accountService,
            ICommissionFeeService commissionFeeService,
            IPublishEndpoint publishEndpoint,
            ILeadRepository leadRepository
        )
        {
            _client = new RestClient(connectionOptions.Value.TransactionStoreUrl);
            _commission = commissionOptions.Value.Commission;
            _vipCommission = commissionOptions.Value.VipCommission;
            _commissionModifier = commissionOptions.Value.CommissionModifier;
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
            _accountService = accountService;
            _commissionFeeService = commissionFeeService;
            _publishEndpoint = publishEndpoint;
            _leadRepository = leadRepository;
        }

        public CommissionFeeDto AddDeposit(TransactionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = _leadRepository.GetLeadById(leadInfo.LeadId);
            var account = CheckAccessAndReturnAccount(model.AccountId, leadInfo);
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
            EmailSender(leadDto, EmailMessages.DepositSubject, string.Format(EmailMessages.DepositBody, model.Amount));
            var dto = new CommissionFeeDto
            { LeadId = leadInfo.LeadId, AccountId = model.AccountId, TransactionId = transactionId, Role = leadInfo.Role, Amount = commission, TransactionType = TransactionType.Deposit };

            AddCommissionFee(dto);

            return dto;
        }

        public CommissionFeeDto AddWithdraw(TransactionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = _leadRepository.GetLeadById(leadInfo.LeadId);
            var accountModel = _accountService.GetAccountWithTransactions(model.AccountId, leadInfo);
            _accountValidationHelper.CheckForVipAccess(accountModel.Currency, leadInfo);
            var commission = CalculateCommission(model.Amount, leadInfo);

            CheckBalance(accountModel, model.Amount);

            model.Amount -= commission;
            model.AccountId = accountModel.Id;
            model.Currency = accountModel.Currency;

            var request = _requestHelper.CreatePostRequest(AddWithdrawEndpoint, model);
            var result = _client.Execute<long>(request);
            var transactionId = result.Data;


            EmailSender(leadDto, EmailMessages.WithdrawSubject, string.Format(EmailMessages.WithdrawBody, model.Amount));

            var dto = new CommissionFeeDto
            { LeadId = leadInfo.LeadId, AccountId = model.AccountId, TransactionId = transactionId, Role = leadInfo.Role, Amount = commission, TransactionType = TransactionType.Withdraw };

            AddCommissionFee(dto);

            return dto;
        }

        public CommissionFeeDto AddTransfer(TransferBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var leadDto = _leadRepository.GetLeadById(leadInfo.LeadId);
            var account = _accountService.GetAccountWithTransactions(model.AccountId, leadInfo);
            var recipientAccount = CheckAccessAndReturnAccount(model.RecipientAccountId, leadInfo);
            var commission = CalculateCommission(model.Amount, leadInfo);

            CheckBalance(account, model.Amount);

            if (account.Currency != Currency.RUB && account.Currency != Currency.USD && !leadInfo.IsVip())
            {
                commission *= _commissionModifier;
                if (account.Balance != model.Amount)
                {
                    throw new ValidationException(nameof(model.Amount), $"{ServiceMessages.IncompleteTransfer}");
                }
            }

            model.Amount -= commission;
            model.Currency = account.Currency;
            model.RecipientCurrency = recipientAccount.Currency;
            var request = _requestHelper.CreatePostRequest(AddTransferEndpoint, model);
            var result = _client.Execute<List<long>>(request);

            if (result.Data == null)
            {
                throw new Exception("tstore slomalsy");
            }
            var transactionId = result.Data.First();
            EmailSender(leadDto, EmailMessages.TransferSubject, string.Format(EmailMessages.TransferBody, model.Amount, model.Currency, model.RecipientCurrency));

            var dto = new CommissionFeeDto
            { LeadId = leadInfo.LeadId, AccountId = model.AccountId, TransactionId = transactionId, Role = leadInfo.Role, Amount = commission, TransactionType = TransactionType.Transfer };

            AddCommissionFee(dto);

            return dto;
        }

        private AccountDto CheckAccessAndReturnAccount(int accountId, LeadIdentityInfo leadInfo)
        {
            var account = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            _accountValidationHelper.CheckLeadAccessToAccount(account.LeadId, leadInfo.LeadId);
            return account;
        }

        private void AddCommissionFee(CommissionFeeDto dto)
        {
            _commissionFeeService.AddCommissionFee(dto);
        }

        private decimal CalculateCommission(decimal amount, LeadIdentityInfo leadInfo)
        {
            return leadInfo.IsVip() ? amount * _vipCommission : amount * _commission;
        }

        private static void CheckBalance(AccountBusinessModel account, decimal amount)
        {
            if (account.Balance - amount < 0)
            {
                throw new ValidationException(nameof(amount), string.Format(ServiceMessages.DoesNotHaveEnoughMoney, account.Id, account.Balance));
            }
        }

        private void EmailSender(LeadDto dto, string subject, string body)
        {
            _publishEndpoint.Publish<IMailExchangeModel>(new
            {
                Subject = subject,
                Body = $"{dto.LastName} {dto.FirstName} {body}",
                DisplayName = "Best CRM",
                MailAddresses = $"{dto.Email}"
            });
        }
    }
}