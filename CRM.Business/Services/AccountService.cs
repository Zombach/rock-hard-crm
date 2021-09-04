using System;
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
using System.Collections.Generic;
using System.Linq;
using CRM.DAL.Enums;
using Newtonsoft.Json.Serialization;
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
            _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadId);

            _accountRepository.DeleteAccount(id);
        }

        public AccountBusinessModel GetAccountWithTransactions(int accountId, LeadIdentityInfo leadInfo)
        {
            JsonSerializerSettings aaa = new()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            var dto = _accountValidationHelper.GetAccountByIdAndThrowIfNotFound(accountId);
            if (!leadInfo.IsAdmin())
                _accountValidationHelper.CheckLeadAccessToAccount(dto.LeadId, leadInfo.LeadId);

            var accountModel = _mapper.Map<AccountBusinessModel>(dto);
            var request = _requestHelper.CreateGetRequest($"{GetTransactionsByAccountIdEndpoint}{accountId}");

            var response = _client.Execute<string>(request);
            var transfers = new List<TransferBusinessModel>();
            var transactions = new List<TransactionBusinessModel>();

            var json = response.Data.Replace("TransactionStore.DAL.Models.TransferDto, TransactionStore.DAL", "CRM.Business.Models.TransferBusinessModel, CRM.Business"); 
            json = json.Replace("TransactionStore.DAL.Models.TransactionDto, TransactionStore.DAL", "CRM.Business.Models.TransactionBusinessModel, CRM.Business");
            var result = JsonConvert.DeserializeObject<List<object>>(json,aaa);
            if (result!=null)
                foreach (var obj in result)
                {
                    if (obj is TransferBusinessModel model)
                    {
                        transfers.Add(model);
                        if (model.AccountId == accountId)
                        {
                            accountModel.Balance += model.Amount;
                        }
                    }
                    else
                    {
                        var mode = (TransactionBusinessModel)obj;
                        transactions.Add(mode);
                        accountModel.Balance += mode.Amount;
                    }

                }
            accountModel.Transactions = transactions;
            accountModel.Transfers = transfers;

            return accountModel;
        }
    }

    //public class KnownTypesBinder : ISerializationBinder
    //{
    //    public IList<Type> KnownTypes { get; set; }

    //    public Type BindToType(string assemblyName, string typeName)
    //    {
    //        return KnownTypes.SingleOrDefault(t => t.Name == typeName);
    //    }

    //    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    //    {
    //        assemblyName = null;
    //        typeName = serializedType.Name;
    //    }
    //}
}