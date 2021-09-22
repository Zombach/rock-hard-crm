using CRM.Business.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public partial class AccountService
    {
        private int _maxSizeList = 20000;

        private static readonly Dictionary<string, List<TransferBusinessModel>> _transfers = new();
        private static readonly Dictionary<string, List<TransactionBusinessModel>> _transactions = new();

        public async Task<T> AddDeserializedTransactionsAsync<T>(T model, string json, string leadId) where T : class
        {
            JToken jToken;

            switch (model)
            {
                case List<AccountBusinessModel> when json != "[]":
                    jToken = GetJToken(json);
                    await GetListModels(jToken, leadId);
                    break;
                case List<AccountBusinessModel> businessModels:
                    {
                        var listIds = new List<int>();
                        listIds.AddRange(_transactions[leadId].Select(item => item.AccountId).Distinct());
                        listIds.AddRange(_transfers[leadId].Select(item => item.AccountId).Distinct());
                        List<int> ids = listIds.Distinct().ToList();
                        ids.Sort();
                        _transactions[leadId] = _transactions[leadId].OrderBy(t => t.AccountId).ToList();
                        _transfers[leadId] = _transfers[leadId].OrderBy(t => t.AccountId).ToList();
                        businessModels = await GetAccountsInfoAsync(ids);
                        businessModels = businessModels.OrderBy(b => b.Id).ToList();

                        return await GroupsTransactionsAndTransfers(businessModels, leadId) as T;
                    }
                case AccountBusinessModel businessModel:
                    {
                        jToken = GetJToken(json);
                        await GetListModels(jToken, leadId);
                        if (jToken == null)
                        {
                            throw new Exception("tstore slomalsya");
                        }
                        await GetListModels(jToken, leadId);

                        businessModel.Transactions = _transactions[leadId];
                        businessModel.Transfers = _transfers[leadId];
                        return businessModel as T;
                    }
            }

            return model;
        }

        private async Task<List<AccountBusinessModel>> GroupsTransactionsAndTransfers(List<AccountBusinessModel> accounts, string leadId)
        {
            int i = 0;
            List<TransferBusinessModel> tmpTransfer = new();
            List<TransactionBusinessModel> tmpTransaction = new();
            do
            {
                GetValuesForList(tmpTransfer, out tmpTransfer, leadId);
                GetValuesForList(tmpTransaction, out tmpTransaction, leadId);

                if (tmpTransaction.Count != 0 && tmpTransfer.Count != 0)
                {
                    for (; i < accounts.Count; i++)
                    {
                        await SetValuesForAccount(accounts[i], tmpTransfer, tmpTransaction);
                        if (tmpTransaction.Count == 0 || tmpTransfer.Count == 0) { break; }
                    }
                }


                if (tmpTransaction.Count != 0 && tmpTransfer.Count != 0) continue;
                if (_transactions[leadId].Count != 0 || _transfers[leadId].Count != 0) continue;
                for (; i < accounts.Count; i++)
                {
                    if (tmpTransaction.Count == 0)
                    {
                        GetTransfersForAccount(accounts[i], tmpTransfer);
                    }
                    else
                    {
                        GetTransactionsForAccount(accounts[i], tmpTransaction);
                    }
                }

                if (i == accounts.Count && tmpTransaction.Count != 0 || i == accounts.Count && tmpTransfer.Count != 0) { throw new Exception("Что-то пошло не по плану"); }
            }
            while (tmpTransaction.Count != 0 || tmpTransfer.Count != 0);

            return accounts;
        }

        private void GetValuesForList<T>(T models, out T result, string leadId) where T : class
        {
            if (models is List<TransferBusinessModel> transfers)
            {
                GetTransfersForList(transfers, out transfers, leadId);
                result = transfers as T;
            }
            else if (models is List<TransactionBusinessModel> transaction)
            {
                GetTransactionForList(transaction, out transaction, leadId);
                result = transaction as T;
            }
            else
            {
                throw new Exception("Не удалось определить тип данных");
            }
        }

        private async Task SetValuesForAccount(AccountBusinessModel account, List<TransferBusinessModel> transfers, List<TransactionBusinessModel> transactions)
        {
            List<Task> tasks = new()
            {
                Task.Run(() =>
                {
                    GetTransactionsForAccount(account, transactions);
                }),
                Task.Run(() =>
                {
                    GetTransfersForAccount(account, transfers);
                })
            };
            await Task.WhenAll(tasks);
        }

        private void GetTransfersForList(List<TransferBusinessModel> transfers, out List<TransferBusinessModel> result, string leadId)
        {
            if (transfers.Count == 0)
            {
                if (_transfers.Count != 0)
                {
                    var transfersCount = _transfers[leadId].Count >= _maxSizeList ? _maxSizeList : _transfers[leadId].Count;
                    transfers = _transfers[leadId].GetRange(0, transfersCount);
                    _transfers[leadId].RemoveRange(0, transfersCount);
                }
            }
            result = transfers;
        }

        private void GetTransactionForList(List<TransactionBusinessModel> transactions, out List<TransactionBusinessModel> result, string leadId)
        {
            if (transactions.Count == 0)
            {
                if (_transactions.Count != 0)
                {
                    int transactionsCount = _transactions[leadId].Count >= _maxSizeList ? _maxSizeList : _transactions[leadId].Count;
                    transactions = _transactions[leadId].GetRange(0, transactionsCount);
                    _transactions[leadId].RemoveRange(0, transactionsCount);
                }
            }
            result = transactions;
        }

        private void GetTransfersForAccount(AccountBusinessModel account, List<TransferBusinessModel> tmpTransfer)
        {
            account.Transfers = tmpTransfer.FindAll(t => t.AccountId == account.Id);
            tmpTransfer.RemoveAll(t => t.AccountId == account.Id);
        }

        private void GetTransactionsForAccount(AccountBusinessModel account, List<TransactionBusinessModel> tmpTransactions)
        {
            account.Transactions = tmpTransactions.FindAll(t => t.AccountId == account.Id);
            tmpTransactions.RemoveAll(t => t.AccountId == account.Id);
        }

        private T BalanceCalculation<T>(T model, int accountId)
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

        private JToken GetJToken(string json)
        {
            try
            {
                return JArray.Parse(json);
            }
            catch
            {
                throw new Exception("все упало");
            }
        }

        private async Task GetListModels(JToken jToken, string leadId)
        {
            List<Task> tasks = new()
            {
                Task.Run(() =>
                {
                    List<TransferBusinessModel> transfersJToken = jToken.Where(j => j.SelectToken(@"$.RecipientAccountId") != null)
                        .Select(t => t.ToObject<TransferBusinessModel>()).ToList();
                    if (_transfers.ContainsKey(leadId))
                    {
                        _transfers[leadId].AddRange(transfersJToken);
                    }
                    else
                    {
                        _transfers.Add(leadId, transfersJToken);
                    }

                }),
                Task.Run(() =>
                {
                    List<TransactionBusinessModel> transactionsJToken = jToken.Where(j => j.SelectToken(@"$.RecipientAccountId") == null)
                        .Select(t => t.ToObject<TransactionBusinessModel>()).ToList();
                    if (_transactions.ContainsKey(leadId))
                    {
                        _transactions[leadId].AddRange(transactionsJToken);
                    }
                    else
                    {
                        _transactions.Add(leadId, transactionsJToken);
                    }
                })
            };
            await Task.WhenAll(tasks);
        }

        private void GetBalanceModel(AccountBusinessModel model, int accountId)
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

        private async Task<List<AccountBusinessModel>> GetAccountsInfoAsync(List<int> ids)
        {
            var dto = await _accountRepository.GetAccountsByListIdAsync(ids);
            return _mapper.Map<List<AccountBusinessModel>>(dto);
        }

        private void CleanListModels(string leadId)
        {
            if (_transfers.ContainsKey(leadId))
            {
                _transfers.Remove(leadId);
            }
            if (_transactions.ContainsKey(leadId))
            {
                _transactions.Remove(leadId);
            }
        }
    }
}