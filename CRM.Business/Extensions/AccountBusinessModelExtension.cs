using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CRM.DAL.Repositories;

namespace CRM.Business.Models
{
    public static class AccountBusinessModelExtension
    {
        public static bool IsPart { get; set; }
        public static List<TransferBusinessModel> Transfers;
        public static List<TransactionBusinessModel> Transactions;

        public static T AddDeserializedTransactions<T>(this T model, string json, IAccountRepository accountRepository, IMapper mapper)
        {
            JToken jToken;

            if (json == string.Empty)
            {
                throw new Exception();//транзакций в периоде нет
            }

            if (model is List<AccountBusinessModel> businessModels)
            {
                jToken = CheckStatusGetJToken(json);
                if (jToken == null)
                {
                    throw new Exception("tstore slomalsya");
                }

                GetListModels(jToken, Transfers, Transactions);
                if (IsPart) { return model; }

                var listIds = new List<int>();
                listIds.AddRange(Transactions.Select(item => item.AccountId).Distinct());
                listIds.AddRange(Transfers.Select(item => item.AccountId).Distinct());
                var ids = listIds.Distinct().ToList();
                ids.Sort();
                //Transactions = Transactions.OrderBy(t => t.AccountId).ToList();
                //Transfers = Transfers.OrderBy(t => t.AccountId).ToList();
                businessModels.GetAccountsInfo(ids, accountRepository, mapper);

                var b = DateTime.Now;
                foreach (var item in businessModels)
                {
                    item.Transactions = Transactions.FindAll(t => t.AccountId == item.Id);
                    Transactions.RemoveAll(t=>t.AccountId==item.Id);
                    item.Transfers = Transfers.FindAll(t => t.AccountId == item.Id);
                    Transfers.RemoveAll(t => t.AccountId == item.Id);
                }

                var c = DateTime.Now;
                var v = c - b;
            }

            if (model is AccountBusinessModel businessModel)
            {
                jToken = CheckStatusGetJToken(json);
                if (jToken == null)
                {
                    throw new Exception("tstore slomalsya");
                }
                GetListModels(jToken, Transfers, Transactions);

                businessModel.Transactions = Transactions;
                businessModel.Transfers = Transfers;
            }

            return model;
        }

        public static T BalanceCalculation<T>(this T model, int accountId)
        {
            if (model is List<AccountBusinessModel> businessModels)
            {
                foreach (var item in businessModels)
                {
                    GetBalanceModel(item, accountId);
                }
            }
            if (model is AccountBusinessModel businessModel)
            {
                GetBalanceModel(businessModel, accountId);
            }

            return model;
        }

        private static JToken CheckStatusGetJToken(string json)
        {
            var jObject = JObject.Parse(json);
            var status = jObject.SelectToken(@"$.Status");
            if (status != null) IsPart = status.ToObject<bool>();
            return jObject.SelectToken(@"$.List");
        }

        private static JToken GetJToken(string json)
        {
            try
            {
                return JObject.Parse(json);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        private static void GetListModels(JToken jToken, List<TransferBusinessModel> transfers, List<TransactionBusinessModel> transactions)
        {
            var transfersJToken = jToken.Where(j => j.SelectToken(@"$.RecipientAccountId") != null)
                .Select(t => t.ToObject<TransferBusinessModel>()).ToList();
            var transactionsJToken = jToken.Where(j => j.SelectToken(@"$.RecipientAccountId") == null)
                .Select(t => t.ToObject<TransactionBusinessModel>()).ToList();

            transactions.AddRange(transactionsJToken);
            transfers.AddRange(transfersJToken);
        }

        private static void GetBalanceModel(AccountBusinessModel model, int accountId)
        {
            if (model.Transactions != null)
            {
                foreach (var obj in model.Transactions)
                {
                    model.Balance += obj.Amount;
                }
            }

            if (model.Transfers != null)
            {
                foreach (var obj in model.Transfers.Where(obj => obj.AccountId == accountId))
                {
                    model.Balance += obj.Amount;
                }
            }
        }

        private static void GetAccountsInfo(this List<AccountBusinessModel> list, List<int> ids, IAccountRepository accountRepository, IMapper mapper)
        {
            var idss = new List<int>();
            foreach (var item in ids)
            {
                var account = accountRepository.GetAccountById(item);
                if (account == null)
                {
                    idss.Add(item);//errror
                }
                else
                {
                    var model = mapper.Map<AccountBusinessModel>(account);
                    list.Add(model);
                }
            }
        }
    }
}