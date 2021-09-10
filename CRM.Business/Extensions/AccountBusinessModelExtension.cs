using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Business.Models
{
    public static class AccountBusinessModelExtension
    {
        private const string _recipientId = @"$.[?(@.RecipientAccountId)]";
        private static List<TransferBusinessModel> _transfers { get; set; }
        private static List<TransactionBusinessModel> _transactions { get; set; }

        public static T AddDeserializedTransactions<T>(this T model, string json)
        {
            GetListModels(json);

            if (model is List<AccountBusinessModel> businessModels)
            {
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
        
        private static void GetListModels(string json)
        {
            var jArray = JArray.Parse(json);
            _transfers = jArray.Where(j => j.SelectToken(_recipientId) != null)
                .Select(t => t.ToObject<TransferBusinessModel>()).ToList();
            _transactions = jArray.Where(j => j.SelectToken(_recipientId) == null)
                .Select(t => t.ToObject<TransactionBusinessModel>()).ToList();
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