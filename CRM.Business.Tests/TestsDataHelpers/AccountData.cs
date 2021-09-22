using System;
using System.Collections.Generic;
using System.Xml.Schema;
using CRM.Business.Models;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Newtonsoft.Json;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public static class AccountData
    {
        public static AccountDto GetUsdAccountDto()
        {
            return new()
            {
                Id = 1,
                LeadId = 1,
                Currency = Currency.USD,
                CreatedOn = DateTime.Now.AddYears(-1),
                IsDeleted = false
            };
        }

        public static AccountDto GetRubAccountDto()
        {
            return new()
            {
                Id = 1,
                LeadId = 1,
                Currency = Currency.RUB,
                CreatedOn = DateTime.Now.AddYears(-1),
                IsDeleted = false
            };
        }

        public static AccountDto GetEurAccountDto()
        {
            return new()
            {
                Id = 2,
                LeadId = 2,
                Currency = Currency.EUR,
                CreatedOn = DateTime.Now.AddYears(-1),
                IsDeleted = false
            };
        }

        public static List<AccountDto> GetListAccountDtos()
        {
            return new()
            {
                new()
                {
                    Id = 2,
                    LeadId = 2,
                    Currency = Currency.RUB,
                    CreatedOn = DateTime.Now.AddYears(-1),
                    IsDeleted = false
                },
                new()
                {
                    Id = 652731,
                    LeadId = 652731,
                    Currency = Currency.USD,
                    CreatedOn = DateTime.Now.AddYears(-1),
                    IsDeleted = false
                },
                new()
                {
                    Id = 1958795,
                    LeadId = 1958795,
                    Currency = Currency.USD,
                    CreatedOn = DateTime.Now.AddYears(-1),
                    IsDeleted = false
                }
            };
        }

        public static List<AccountBusinessModel> GetListAccountBusinessModels()
        {
            return new()
            {
                new()
                {
                    Id = 2,
                    LeadId = 2,
                    Currency = Currency.RUB,
                    CreatedOn = DateTime.Now.AddYears(-1),
                    IsDeleted = false
                },
                new()
                {
                    Id = 652731,
                    LeadId = 652731,
                    Currency = Currency.USD,
                    CreatedOn = DateTime.Now.AddYears(-1),
                    IsDeleted = false
                },
                new()
                {
                    Id = 1958795,
                    LeadId = 1958795,
                    Currency = Currency.USD,
                    CreatedOn = DateTime.Now.AddYears(-1),
                    IsDeleted = false
                }
            };
        }

        public static string GetData()
        {
            return
                "[{\"Id\":15432545,\"TransactionType\":2,\"Date\":\"2021-09-21T20:06:49.78\",\"AccountId\":2,\"Amount\":500.000,\"Currency\":1}," +
                "{\"Id\":15432546,\"TransactionType\":2,\"Date\":\"2021-09-21T20:08:30.48\",\"AccountId\":2,\"Amount\":500.000,\"Currency\":1}," +
                "{\"RecipientAccountId\":3,\"RecipientAmount\":100.000,\"RecipientCurrency\":2,\"Id\":15432547,\"TransactionType\":3,\"" +
                "Date\":\"2021-09-21T20:21:45.8866667\",\"AccountId\":2,\"Amount\":-500.000,\"Currency\":1}," +
                "{\"RecipientAccountId\":652731,\"RecipientAmount\":0.931,\"RecipientCurrency\":3,\"Id\":15432549,\"TransactionType\":3,\"" +
                "Date\":\"2021-09-22T15:33:41.9366667\",\"AccountId\":2,\"Amount\":-80.000,\"Currency\":1}," +
                "{\"Id\":15432551,\"TransactionType\":1,\"Date\":\"2021-09-22T16:05:54.9833333\",\"AccountId\":2,\"Amount\":4000.000,\"Currency\":1}," +
                "{\"Id\":15432552,\"TransactionType\":1,\"Date\":\"2021-09-22T16:07:30.9633333\",\"AccountId\":2,\"Amount\":4000.000,\"Currency\":1}," +
                "{\"Id\":15432556,\"TransactionType\":1,\"Date\":\"2021-09-22T17:44:12.43\",\"AccountId\":1958795,\"Amount\":4500.000,\"Currency\":2}," +
                "{\"Id\":15432557,\"TransactionType\":1,\"Date\":\"2021-09-22T18:51:55.12\",\"AccountId\":2,\"Amount\":4000.000,\"Currency\":1}," +
                "{\"Id\":15432558,\"TransactionType\":1,\"Date\":\"2021-09-22T18:53:38.9166667\",\"AccountId\":2,\"Amount\":4000.000,\"Currency\":1}," +
                "{\"Id\":15432559,\"TransactionType\":1,\"Date\":\"2021-09-22T18:54:39.6766667\",\"AccountId\":2,\"Amount\":4000.000,\"Currency\":1}," +
                "{\"Id\":15432560,\"TransactionType\":1,\"Date\":\"2021-09-22T18:54:45.87\",\"AccountId\":2,\"Amount\":4000.000,\"Currency\":1}," +
                "{\"Id\":15432561,\"TransactionType\":2,\"Date\":\"2021-09-22T19:20:55.2233333\",\"AccountId\":2,\"Amount\":80.000,\"Currency\":1}," +
                "{\"RecipientAccountId\":2,\"RecipientAmount\":6873.124,\"RecipientCurrency\":1,\"Id\":15432562,\"TransactionType\":3,\"" +
                "Date\":\"2021-09-22T19:21:14.6333333\",\"AccountId\":652731,\"Amount\":-80.000,\"Currency\":3}," +
                "{\"Id\":15432564,\"TransactionType\":2,\"Date\":\"2021-09-22T19:24:36.6033333\",\"AccountId\":2,\"Amount\":80.000,\"Currency\":1}," +
                "{\"Id\":15432565,\"TransactionType\":2,\"Date\":\"2021-09-22T19:32:02.7633333\",\"AccountId\":2,\"Amount\":80.000,\"Currency\":1}," +
                "{\"Id\":15432566,\"TransactionType\":1,\"Date\":\"2021-09-22T21:25:31.94\",\"AccountId\":2,\"Amount\":4000.000,\"Currency\":1}," +
                "{\"Id\":15432567,\"TransactionType\":2,\"Date\":\"2021-09-22T21:25:56.24\",\"AccountId\":2,\"Amount\":80.000,\"Currency\":1}]";
        }

        public static List<AccountBusinessModel> GetSthngStrange() => JsonConvert.DeserializeObject<List<AccountBusinessModel>>("[{\"Id\":2,\"LeadId\":2,\"Currency\":3,\"CreatedOn\":\"" +
            "2020-09-23T01:25:56.9574384+03:00\",\"Closed\":null,\"IsDeleted\":false,\"Transactions\":[{\"Id\":15432545,\"AccountId\":2,\"Currency\":1,\"TransactionType\":2,\"Date\":\"" +
            "2021-09-21T20:06:49.78\",\"Amount\":500.0},{\"Id\":15432546,\"AccountId\":2,\"Currency\":1,\"TransactionType\":2,\"Date\":\"2021-09-21T20:08:30.48\",\"Amount\":500.0},{\"Id\":1" +
            "5432551,\"AccountId\":2,\"Currency\":1,\"TransactionType\":1,\"Date\":\"2021-09-22T16:05:54.9833333\",\"Amount\":4000.0},{\"Id\":15432552,\"AccountId\":2,\"Currency\":1,\"TransactionType\"" +
            ":1,\"Date\":\"2021-09-22T16:07:30.9633333\",\"Amount\":4000.0},{\"Id\":15432557,\"AccountId\":2,\"Currency\":1,\"TransactionType\":1,\"Date\":\"2021-09-22T18:51:55.12\",\"Amount\":400" +
            "0.0},{\"Id\":15432558,\"AccountId\":2,\"Currency\":1,\"TransactionType\":1,\"Date\":\"2021-09-22T18:53:38.9166667\",\"Amount\":4000.0},{\"Id\":15432559,\"AccountId\":2,\"Currency\":1," +
            "\"TransactionType\":1,\"Date\":\"2021-09-22T18:54:39.6766667\",\"Amount\":4000.0},{\"Id\":15432560,\"AccountId\":2,\"Currency\":1,\"TransactionType\":1,\"Date\":\"2021-09-22T18:54:45.8" +
            "7\",\"Amount\":4000.0},{\"Id\":15432561,\"AccountId\":2,\"Currency\":1,\"TransactionType\":2,\"Date\":\"2021-09-22T19:20:55.2233333\",\"Amount\":80.0},{\"Id\":15432564,\"AccountId\":2," +
            "\"Currency\":1,\"TransactionType\":2,\"Date\":\"2021-09-22T19:24:36.6033333\",\"Amount\":80.0},{\"Id\":15432565,\"AccountId\":2,\"Currency\":1,\"TransactionType\":2,\"Date\":\"2021-09-" +
            "22T19:32:02.7633333\",\"Amount\":80.0},{\"Id\":15432566,\"AccountId\":2,\"Currency\":1,\"TransactionType\":1,\"Date\":\"2021-09-22T21:25:31.94\",\"Amount\":4000.0},{\"Id\":15432567,\"Ac" +
            "countId\":2,\"Currency\":1,\"TransactionType\":2,\"Date\":\"2021-09-22T21:25:56.24\",\"Amount\":80.0}],\"Transfers\":[{\"RecipientTransactionId\":0,\"RecipientAccountId\":3,\"Recipient" +
            "Amount\":100.0,\"RecipientCurrency\":2,\"Id\":15432547,\"AccountId\":2,\"Currency\":1,\"TransactionType\":3,\"Date\":\"2021-09-21T20:21:45.8866667\",\"Amount\":-500.0},{\"RecipientTransactionId\"" +
            ":0,\"RecipientAccountId\":652731,\"RecipientAmount\":0.931,\"RecipientCurrency\":3,\"Id\":15432549,\"AccountId\":2,\"Currency\":1,\"TransactionType\":3,\"Date\":\"2021-09-22T15:33:41.9366667\",\"" +
            "Amount\":-80.0}],\"Balance\":0.0},{\"Id\":652731,\"LeadId\":652731,\"Currency\":1,\"CreatedOn\":\"2020-09-23T01:25:56.9575477+03:00\",\"Closed\":null,\"IsDeleted\":false,\"Transactions\":[]," +
            "\"Transfers\":[{\"RecipientTransactionId\":0,\"RecipientAccountId\":2,\"RecipientAmount\":6873.124,\"RecipientCurrency\":1,\"Id\":15432562,\"AccountId\":652731,\"Currency\":3,\"TransactionType\"" +
            ":3,\"Date\":\"2021-09-22T19:21:14.6333333\",\"Amount\":-80.0}],\"Balance\":0.0},{\"Id\":1958795,\"LeadId\":1958795,\"Currency\":1,\"CreatedOn\":\"2020-09-23T01:25:56.9575488+03:00\",\"Closed\":nu" +
            "ll,\"IsDeleted\":false,\"Transactions\":[{\"Id\":15432556,\"AccountId\":1958795,\"Currency\":2,\"TransactionType\":1,\"Date\":\"2021-09-22T17:44:12.43\",\"Amount\":4500.0}],\"Transfers\":null,\"Balance\":0.0}]");
    }
}