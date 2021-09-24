using System;
using System.Collections.Generic;
using CRM.DAL.Models;

namespace CRM.Business.Tests.TestsDataHelpers
{
    public class AccountDtoComparer : IEqualityComparer<AccountDto>
    {
        public bool Equals(AccountDto x, AccountDto y)
        {
            return x.Currency == y.Currency;
        }

        public int GetHashCode(AccountDto obj)
        {
            throw new NotImplementedException();
        }
    }
}
