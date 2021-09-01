using System;
using CRM.Business.Models;
using CRM.DAL.Enums;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public static class AccountBusinessModelData
    {
        public static AccountBusinessModel GetAccountBusinessModel()
        {
            return new()
            {
                Id = 1,
                LeadId = 1,
                Currency = Currency.RUB,
                CreatedOn = DateTime.Now.AddYears(-1),
                IsDeleted = false,
                Transactions = TransactionBusinessModelData.GetListTransactionBusinessModel(),
                Balance = 1000
            };
        }
    }
}