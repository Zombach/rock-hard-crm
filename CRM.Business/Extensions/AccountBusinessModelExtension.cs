using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Business.Models
{
    public static class AccountBusinessModelExtension
    {
        public static bool IsPart { get; set; }

        public static T AddDeserializedTransactions<T>(this T model, string json)
        {
            List<TransferBusinessModel> _transfers = new();
            List<TransactionBusinessModel> _transactions = new();
            JToken jToken;

            if (json==string.Empty)
            {
                throw new Exception();
            }

            if (model is List<AccountBusinessModel> businessModels)
            {
                jToken = CheckStatusGetJToken(json);
                if (jToken == null)
                {
                    throw new Exception("tstore slomalsya");
                }

                GetListModels(jToken, _transfers, _transactions);
                if (IsPart) { return model; }

                var listIds = new List<int>();
                listIds.AddRange(_transactions.Select(item => item.AccountId).Distinct());
                listIds.AddRange(_transfers.Select(item => item.AccountId).Distinct());
                var ids = listIds.Distinct().ToList();
                businessModels.AddRange(ids.Select(item => new AccountBusinessModel { Id = item }));

                foreach (var item in businessModels)
                {
                    item.Transactions = _transactions.FindAll(t => t.AccountId == item.Id);
                    item.Transfers = _transfers.FindAll(t => t.AccountId == item.Id);
                }
            }

            if (model is AccountBusinessModel businessModel)
            {
                jToken = GetJToken(json);
                if (jToken == null)
                {
                    throw new Exception("tstore slomalsya");
                }
                GetListModels(jToken, _transfers, _transactions);

                businessModel.Transactions = _transactions;
                businessModel.Transfers = _transfers;
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

            transfers.AddRange(transfersJToken);
            transactions.AddRange(transactionsJToken);
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
    }
}