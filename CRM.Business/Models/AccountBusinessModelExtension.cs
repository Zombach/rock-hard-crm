using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Business.Models
{
    public static class AccountBusinessModelExtension
    {
        private const string _recipientId = "RecipientAccountId";
        private static string _transfersJson;
        private static string _transactionsJson;

        public static AccountBusinessModel AddDeserializedTransactions(this AccountBusinessModel model, string json)
        {
            _transfersJson = string.Empty;
            _transactionsJson = string.Empty;

            while (true)
            {
                var start = json.IndexOf("{", StringComparison.Ordinal);
                var end = json.IndexOf("}", StringComparison.Ordinal);

                if (start == -1 && end == -1) break;

                var line = json.Substring(start, end);
                json = json.Substring(end + 1);
                Writer(line);
            }
            FinishWriter();

            model.Transactions = JsonConvert.DeserializeObject<List<TransactionBusinessModel>>(_transactionsJson);
            model.Transfers = JsonConvert.DeserializeObject<List<TransferBusinessModel>>(_transfersJson);
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

        private static void FinishWriter()
        {
            if (_transfersJson.Length > 0)
            {
                _transfersJson = $"[{_transfersJson}]";
            }
            if (_transactionsJson.Length > 0)
            {
                _transactionsJson = $"[{_transactionsJson}]";
            }
        }

        private static void Writer(string line)
        {
            if (line.Contains(_recipientId))
            {
                _transfersJson = _transfersJson.Length > 0 ? $"{_transfersJson},{line}" : line;
            }
            else
            {
                _transactionsJson = _transactionsJson.Length > 0 ? $"{_transactionsJson},{line}" : line;
            }
        }
    }
}