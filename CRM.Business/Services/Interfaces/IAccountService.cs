using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.DAL.Models;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        int AddAccount(AccountDto dto, LeadIdentityInfo leadInfo);
        void DeleteAccount(int accountId, int leadId);
        void RestoreAccount(int accountId, int leadId);
        AccountBusinessModel GetAccountWithTransactions(int accountId, LeadIdentityInfo leadInfo);
        List<AccountBusinessModel> GetTransactionsByPeriodAndPossiblyAccountId(TimeBasedAcquisitionBusinessModel model, LeadIdentityInfo leadInfo);
        AccountBusinessModel GetLeadBalance(int leadId, LeadIdentityInfo leadInfo);
    }
}