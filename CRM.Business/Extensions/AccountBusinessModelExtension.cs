using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace CRM.Business.Models
{
    public static class AccountBusinessModelExtension
    {
        private const string _recipientId = "$..RecipientAccountId";
        private const string _recipientIds = "$...RecipientAccountId";

        public static AccountBusinessModel AddDeserializedTransactions(this AccountBusinessModel model, string json)
        {
            return aaa(model, json,_recipientId);
        }

        public static List<AccountBusinessModel> AddDeserializedTransactions(this List<AccountBusinessModel> model, string json)
        {
            return aaa(model, json, _recipientIds);
        }

        public static T aaa<T>(T model, string json, string a)
        {
            var jArray = JArray.Parse(json);
            var _transfersJson = jArray.SelectTokens(@"$.[?(@.RecipientAccountId)]").ToList();
            var _transactionsJson = jArray.ToList();

            foreach (var item in _transfersJson)
            {
                _transactionsJson.Remove(item);
            }

            var transactions = _transactionsJson.Select(item => item.ToObject<TransactionBusinessModel>()).ToList();
            var transfers = _transfersJson.Select(item => item.ToObject<TransferBusinessModel>()).ToList();

            if (model is List<AccountBusinessModel> d)
            {
                var listIds = new List<int>();
                if (transactions != null) listIds.AddRange(transactions.Select(item => item.AccountId).Distinct());
                //if (transactions != null) listIds.AddRange(transactions.Select(item => item.AccountId).Distinct());
                if (transfers != null) listIds.AddRange(transfers.Select(item => item.AccountId).Distinct());
                d.AddRange(listIds.Select(item => new AccountBusinessModel { Id = item }));

                foreach (var item in d)
                {
                    if (transactions != null) item.Transactions = transactions.FindAll(t => t.AccountId == item.Id);
                    if (transfers != null) item.Transfers = transfers.FindAll(t => t.AccountId == item.Id);
                }
            }

            if (model is AccountBusinessModel b)
            {
                b.Transactions = transactions;
                b.Transfers = transfers;
            }
            
            return model;
        }

        public static AccountBusinessModel BalanceCalculation(this AccountBusinessModel model, int accountId)
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

        //public static List<AccountBusinessModel> AddDeserializedTransactionsForList(this List<AccountBusinessModel> model, string json)
        //{
        //    _transfersJson = string.Empty;
        //    _transactionsJson = string.Empty;

        //    while (true)
        //    {
        //        var start = json.IndexOf("{", StringComparison.Ordinal);
        //        var end = json.IndexOf("}", StringComparison.Ordinal);

        //        if (start == -1 && end == -1) break;

        //        var line = json.Substring(start, end);
        //        json = json.Substring(end + 1);
        //        Writer(line);
        //    }
        //    FinishWriter();

        //    var transactions = JsonConvert.DeserializeObject<List<TransactionBusinessModel>>(_transactionsJson);
        //    var transfers = JsonConvert.DeserializeObject<List<TransferBusinessModel>>(_transfersJson);

        //    var listIds = new List<int>();
        //    if (transactions != null) listIds.AddRange(transactions.Select(item => item.AccountId));
        //    if (transfers != null) listIds.AddRange(transfers.Select(item => item.AccountId));
        //    model.AddRange(listIds.Select(item => new AccountBusinessModel { Id = item }));

        //    foreach (var item in model)
        //    {
        //        if (transactions != null) item.Transactions = transactions.FindAll(t => t.AccountId == item.Id);
        //        if (transfers != null) item.Transfers = transfers.FindAll(t => t.AccountId == item.Id);
        //    }

        //    return model;
        //}
    }
}