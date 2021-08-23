using System.Collections.Generic;
using CRM.Business.Models;
using CRM.DAL.Models;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        int AddAccount(AccountDto dto);
        void DeleteAccount(int id);
        List<TransactionModel> GetTransactionsByAccountId(int id);
        long AddDeposit(TransactionModel model);
    }
}