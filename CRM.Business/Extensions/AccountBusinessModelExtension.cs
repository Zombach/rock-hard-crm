using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Business.Models
{
    public static class AccountBusinessModelExtension
    {
        private const string _recipientId = @"$.[?(@.RecipientAccountId)]";
        private static List<TransferBusinessModel> _transfers;
        private static List<TransactionBusinessModel> _transactions;
        public static bool IsPart { get; set; }

        public static T AddDeserializedTransactions<T>(this T model, string json)
        {
            JToken jToken;
            if (model is List<AccountBusinessModel> businessModels)
            {
                jToken = CheckStatusGetJToken(json);
                GetListModels(jToken);
                if (IsPart) { return model; }

                var listIds = new List<int>();
                if (_transactions != null) listIds.AddRange(_transactions.Select(item => item.AccountId).Distinct());
                if (_transfers != null) listIds.AddRange(_transfers.Select(item => item.AccountId).Distinct());
                var ids = listIds.Distinct().ToList();
                businessModels.AddRange(ids.Select(item => new AccountBusinessModel { Id = item }));

                foreach (var item in businessModels)
                {
                    if (_transactions != null) item.Transactions = _transactions.FindAll(t => t.AccountId == item.Id);
                    if (_transfers != null) item.Transfers = _transfers.FindAll(t => t.AccountId == item.Id);
                }
            }

            if (model is AccountBusinessModel businessModel)
            {
                jToken = GetJToken(json);
                GetListModels(jToken);

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
            IsPart = jObject.SelectToken(@"$.Status").ToObject<bool>();
            return jObject.SelectToken(@"$.List");            
        }
        
        private static JToken GetJToken(string json)
        {
            return JArray.Parse(json);
        }

        private static void GetListModels(JToken jToken)
        {            
            var transfers = jToken.Where(j => j.SelectToken(_recipientId) != null)
                .Select(t => t.ToObject<TransferBusinessModel>()).ToList();
            var transactions = jToken.Where(j => j.SelectToken(_recipientId) == null)
                .Select(t => t.ToObject<TransactionBusinessModel>()).ToList();

            if(!IsPart)
            {
                _transfers = transfers;
                _transactions = transactions;
            }
            else
            {
                _transfers.AddRange(transfers);
                _transactions.AddRange(transactions);                
            }
        }

        private static AccountBusinessModel GetBalanceModel(AccountBusinessModel model, int accountId)
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
            return model;
        }
    }
}