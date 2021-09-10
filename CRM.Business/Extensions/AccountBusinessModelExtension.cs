using System.Collections.Generic;
using System.Linq;

namespace CRM.Business.Models
{
    public static class AccountBusinessModelExtension
    {
        public static List<TransferBusinessModel> Transfers { get; set; }
        public static List<TransactionBusinessModel> Transactions { get; set; }

        public static T AddDeserializedTransactions<T>(this T model, string json)
        {
            AccountBusinessModel.GetListModels(json);

            if (model is List<AccountBusinessModel> businessModels)
            {
                var listIds = new List<int>();
                if (Transactions != null) listIds.AddRange(Transactions.Select(item => item.AccountId).Distinct());
                //if (transactions != null) listIds.AddRange(transactions.Select(item => item.AccountId).Distinct());
                if (Transfers != null) listIds.AddRange(Transfers.Select(item => item.AccountId).Distinct());
                businessModels.AddRange(listIds.Select(item => new AccountBusinessModel { Id = item }));

                foreach (var item in businessModels)
                {
                    if (Transactions != null) item.Transactions = Transactions.FindAll(t => t.AccountId == item.Id);
                    if (Transfers != null) item.Transfers = Transfers.FindAll(t => t.AccountId == item.Id);
                }
            }

            if (model is AccountBusinessModel businessModel)
            {
                businessModel.Transactions = Transactions;
                businessModel.Transfers = Transfers;
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