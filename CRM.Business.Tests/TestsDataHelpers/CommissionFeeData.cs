using System.Collections.Generic;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using NUnit.Framework;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public static class CommissionFeeData
    {
        public static CommissionFeeDto GetCommissionFeeDto()
        {
            return new()
            {
                LeadId = 1,
                AccountId = 1,
                CommissionAmount = 0.2m,
                Role = Role.Regular,
                TransactionId = 213543L,
                TransactionType = TransactionType.Deposit
            };
        }

        public static List<CommissionFeeDto> GetListCommissionFeeDto()
        {
            return new()
            {
                new()
                {
                    LeadId = 1,
                    AccountId = 1,
                    CommissionAmount = 0.2m,
                    Role = Role.Regular,
                    TransactionId = 213543L,
                    TransactionType = TransactionType.Deposit
                },
                new()
                {
                    LeadId = 1,
                    AccountId = 1,
                    CommissionAmount = 0.2m,
                    Role = Role.Regular,
                    TransactionId = 213543L,
                    TransactionType = TransactionType.Deposit
                }
            };
        }
    }
}
