﻿using System;
using System.Collections.Generic;
using CRM.Business.Models;
using CRM.DAL.Enums;
using Newtonsoft.Json;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public static class TransactionData
    {
        public static AccountBusinessModel GetAccountBusinessModel() => new()
        {
            Id = 1,
            LeadId = 1,
            Currency = Currency.RUB,
            CreatedOn = DateTime.Now.AddYears(-1),
            IsDeleted = false,
            Transactions = GetListTransactionBusinessModel(),
            Transfers = GetListTransferBusinessModel(),
            Balance = 1000
        };

        public static AccountBusinessModel GetEurAccountBusinessModel() => new()
        {
            Id = 1,
            LeadId = 1,
            Currency = Currency.EUR,
            CreatedOn = DateTime.Now.AddYears(-1),
            IsDeleted = false,
            Transactions = GetListTransactionBusinessModel(),
            Transfers = GetListTransferBusinessModel(),
            Balance = 1000
        };

        public static TransactionBusinessModel GeTransactionBusinessModel() => new()
        {
            Id = 1,
            AccountId = 1,
            Currency = Currency.RUB,
            TransactionType = TransactionType.Deposit,
            Date = DateTime.Now,
            Amount = decimal.One
        };

        public static TransferBusinessModel GetTransferBusinessModel()
        {
            return new()
            {
                RecipientAccountId = 1,
                RecipientAmount = 12,
                RecipientCurrency = Currency.RUB
            };
        }

        public static TransferBusinessModel GetTransferEurBusinessModel()
        {
            return new()
            {
                RecipientAccountId = 1,
                RecipientAmount = 12,
                RecipientCurrency = Currency.JPY
            };
        }

        public static List<TransactionBusinessModel> GetListTransactionBusinessModel()
        {
            return new()
            {
                new()
                {
                    Id = 1,
                    AccountId = 1,
                    Currency = Currency.RUB,
                    TransactionType = TransactionType.Deposit,
                    Date = DateTime.Now,
                    Amount = 1m
                },
                new()
                {
                    Id = 2,
                    AccountId = 2,
                    Currency = Currency.USD,
                    TransactionType = TransactionType.Deposit,
                    Date = DateTime.Now,
                    Amount = 1m
                }
            };
        }

        public static List<TransferBusinessModel> GetListTransferBusinessModel()
        {
            return new()
            {

            };
        }

        public static string GetJSONstring()
        {
            var list = GetListTransactionBusinessModel();
            return JsonConvert.SerializeObject(list);
        }
    }
}