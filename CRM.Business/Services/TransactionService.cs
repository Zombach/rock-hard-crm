using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using DevEdu.Business.ValidationHelpers;
using Microsoft.Extensions.Options;
using RestSharp;
using System;
using static CRM.Business.TransactionEndpoint;

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
        private const double _commissionModifier = 1.5;

        public TransactionService
        (
            IOptions<ConnectionUrl> connectionOptions,
            IOptions<CommissionSettings> commissionOptions,
            IAccountValidationHelper accountValidationHelper,
            IAccountService accountService, 
            ICommissionFeeService commissionFeeService
        )
        {
            _client = new RestClient(connectionOptions.Value.TstoreUrl);
            _commission = commissionOptions.Value.Commission; 
            _vipCommission = commissionOptions.Value.VipCommission;
            _requestHelper = new RequestHelper();
            _accountValidationHelper = accountValidationHelper;
            _accountService = accountService;
            _commissionFeeService = commissionFeeService;
        }

        public long AddDeposit(TransactionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var account = CheckAccessAndReturnAccount(model.AccountId, leadInfo);
            _accountValidationHelper.CheckForVipAccess(account.Currency, leadInfo);
            var commission = CalculateCommission(model.Amount, leadInfo);

            //if (account.Currency is not Currency.RUB)
            //{
            //    var rate = 1;
            //    commission *= rate;
            //}

            model.Amount -= commission;
            model.AccountId = account.Id;
            model.Currency = account.Currency;

            var request = _requestHelper.CreatePostRequest(AddDepositEndpoint, model);
            var result = _client.Execute<long>(request);
            var transactionId = result.Data;

            AddCommissionFee(leadInfo,transactionId,model, commission);

            return transactionId;
        }

        public long AddWithdraw(TransactionBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var account = CheckAccessAndReturnAccount(model.AccountId, leadInfo);
            _accountValidationHelper.CheckForVipAccess(account.Currency, leadInfo);
            var commission= CalculateCommission(model.Amount, leadInfo);

            model.Amount -= commission;
            model.AccountId = account.Id;
            model.Currency = account.Currency;

            var request = _requestHelper.CreatePostRequest(AddWithdrawEndpoint, model);
            var result = _client.Execute<long>(request);
            var transactionId = result.Data;

            AddCommissionFee(leadInfo, transactionId, model, commission);

            return transactionId;
        }

        public string AddTransfer(TransferBusinessModel model, LeadIdentityInfo leadInfo)
        {
            var account = CheckAccessAndReturnAccount(model.AccountId, leadInfo);
            var recipientAccount = CheckAccessAndReturnAccount(model.RecipientAccountId, leadInfo);

            if (account.Currency is not (Currency.RUB or Currency.USD) && !leadInfo.IsVip())
            {
                var balance = _accountService.GetAccountWithTransactions(account.Id, leadInfo).Balance;
                if (balance != model.Amount)
                {
                    throw new Exception("снять можно только все бабки простак");
                }
            }

            model.Amount = CalculateCommission(model.Amount, leadInfo);
            model.Currency = account.Currency;
            model.RecipientCurrency = recipientAccount.Currency;
            var request = _requestHelper.CreatePostRequest(AddTransferEndpoint, model);
            var result = _client.Execute<string>(request);
            return result.Data;
        }

        private AccountDto CheckAccessAndReturnAccount(int accountId, LeadIdentityInfo leadInfo)
        {
            var account = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            _accountValidationHelper.CheckLeadAccessToAccount(account.LeadId, leadInfo.LeadId);
            return account;
        }

        private void AddCommissionFee(LeadIdentityInfo leadInfo, long transactionId, TransactionBusinessModel model, decimal commission)
        {
            var role = leadInfo.IsVip() ? Role.Vip : Role.Regular;
            var dto = new CommissionFeeDto
                {LeadId = leadInfo.LeadId, AccountId = model.AccountId, TransactionId = transactionId, Role = role, Amount = commission };
            _commissionFeeService.AddCommissionFee(dto);
        }

        private decimal CalculateCommission(decimal amount, LeadIdentityInfo leadInfo)
        {
            return leadInfo.IsVip() ? amount * _vipCommission : amount * _commission;
        }
    }
}