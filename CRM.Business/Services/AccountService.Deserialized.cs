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
        private static List<TransferBusinessModel> _transfers = new();
        private static List<TransactionBusinessModel> _transactions = new();

        private async Task<T> AddDeserializedTransactionsAsync<T>(T model, string json)
        {
            JToken jToken;

            switch (model)
            {
                case List<AccountBusinessModel> when json != "[]":
                    jToken = GetJToken(json);
                    await GetListModels(jToken);
                    break;
                case List<AccountBusinessModel> businessModels:
                    {
                        var listIds = new List<int>();
                        listIds.AddRange(_transactions.Select(item => item.AccountId).Distinct());
                        listIds.AddRange(_transfers.Select(item => item.AccountId).Distinct());
                        var ids = listIds.Distinct().ToList();
                        ids.Sort();
                        _transactions = _transactions.OrderBy(t => t.AccountId).ToList();
                        _transfers = _transfers.OrderBy(t => t.AccountId).ToList();
                        businessModels = businessModels.OrderBy(b => b.Id).ToList();
                        GetAccountsInfo(ids, out List<AccountBusinessModel> models);

                        var b = DateTime.Now;

                        await For(models);


                        foreach (var item in businessModels)
                        {
                            item.Transactions = _transactions.FindAll(t => t.AccountId == item.Id);
                            _transactions.RemoveAll(t => t.AccountId == item.Id);
                            item.Transfers = _transfers.FindAll(t => t.AccountId == item.Id);
                            _transfers.RemoveAll(t => t.AccountId == item.Id);
                        }

                        var c = DateTime.Now;
                        var v = c - b;
                        break;
                    }
                case AccountBusinessModel businessModel:
                    {
                        jToken = GetJToken(json);
                        await GetListModels(jToken);
                        if (jToken == null)
                        {
                            throw new Exception("tstore slomalsya");
                        }
                        await GetListModels(jToken);

                        businessModel.Transactions = _transactions;
                        businessModel.Transfers = _transfers;
                        break;
                    }
            }

            return model;
        }


        private async Task For(List<AccountBusinessModel> accounts)
        {
            List<TransferBusinessModel> tmpTransfer = new();
            List<TransactionBusinessModel> tmpTransaction = new();
            int i = 0;
            do
            {
                if (tmpTransfer.Count == 0)
                {
                    if (_transfers.Count != 0)
                    {
                        int transfersCount = _transfers.Count >= 2000 ? 2000 : _transfers.Count;
                        tmpTransfer = _transfers.GetRange(0, transfersCount);
                        _transfers.RemoveRange(0, transfersCount);
                    }
                }

                if (tmpTransaction.Count == 0)
                {
                    if (_transactions.Count != 0)
                    {
                        int transactionCount = _transactions.Count >= 2000 ? 2000 : _transactions.Count;
                        tmpTransaction = _transactions.GetRange(0, transactionCount);
                        _transactions.RemoveRange(0, transactionCount);
                    }
                }

                if (tmpTransaction.Count != 0 && tmpTransfer.Count != 0)
                {
                    for (; i < accounts.Count; i++)
                    {
                        List<Task> tasks = new()
                        {
                            Task.Run(() =>
                            {
                                GetTransactionsForAccount(accounts[i], tmpTransaction);
                            }),
                            Task.Run(() =>
                            {
                                GetTransfersForAccount(accounts[i], tmpTransfer);
                            })
                        };
                        await Task.WhenAll(tasks);
                        if (tmpTransaction.Count == 0 || tmpTransfer.Count == 0)
                        {
                            break;
                        }
                    }
                }


                if (tmpTransaction.Count == 0 || tmpTransfer.Count == 0)
                {
                    if (_transactions.Count == 0 && _transfers.Count == 0)
                    {
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

                    }
                }

            } while (tmpTransaction.Count != 0 || tmpTransfer.Count != 0);
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
                return JObject.Parse(json); //Уже не нужно, ошибку сюда
            }
        }

        private async Task GetListModels(JToken jToken)
        {
            List<Task> tasks = new()
            {
                Task.Run(() =>
                {
                    List<TransferBusinessModel> transfersJToken = jToken.Where(j => j.SelectToken(@"$.RecipientAccountId") != null)
                        .Select(t => t.ToObject<TransferBusinessModel>()).ToList();
                    _transfers.AddRange(transfersJToken);
                }),
                Task.Run(() =>
                {
                    List<TransactionBusinessModel> transactionsJToken = jToken.Where(j => j.SelectToken(@"$.RecipientAccountId") == null)
                        .Select(t => t.ToObject<TransactionBusinessModel>()).ToList();
                    _transactions.AddRange(transactionsJToken);
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

        private void GetAccountsInfo(List<int> ids, out List<AccountBusinessModel> models)
        {
            models = new();
            List<int> errorIds = new();
            foreach (var id in ids)
            {
                var account = _accountRepository.GetAccountById(id);
                if (account == null)
                {
                    errorIds.Add(id);//error
                }
                else
                {
                    var model = _mapper.Map<AccountBusinessModel>(account);
                    models.Add(model);
                }
            }
        }

        private void CleanListModels()
        {
            _transfers.Clear();
            _transactions.Clear();
        }
    }
}