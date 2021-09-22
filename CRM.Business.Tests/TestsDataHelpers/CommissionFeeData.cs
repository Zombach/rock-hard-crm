using CRM.DAL.Enums;
using CRM.DAL.Models;

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
    }
}
