using AutoMapper;
using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.Business.Requests;
using CRM.Core;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using DevEdu.Business.ValidationHelpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;
using System.Linq;
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

        public int AddAccount(AccountDto dto, LeadIdentityInfo leadInfo)
        {
            var lead = _leadRepository.GetLeadById(leadInfo.LeadId);
            _accountValidationHelper.CheckForDuplicateCurrencies(lead, dto.Currency);
            _accountValidationHelper.CheckForVipAccess(dto.Currency,leadInfo);
            dto.LeadId = lead.Id;
            var accountId = _accountRepository.AddAccount(dto);
            return accountId;
        }

        public void DeleteAccount(int id, int leadId)
        {
            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(id);
            _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadId);

            _accountRepository.DeleteAccount(id);
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
    }
}