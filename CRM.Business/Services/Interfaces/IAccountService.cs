using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using CRM.DAL.Enums;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        Task<int> AddAccountAsync(Currency currency, LeadIdentityInfo leadInfo);
        Task DeleteAccountAsync(int accountId, int leadId);
        Task RestoreAccountAsync(int accountId, int leadId);
        Task<AccountBusinessModel> GetAccountWithTransactionsAsync(int accountId, LeadIdentityInfo leadInfo);
        Task<List<AccountBusinessModel>> GetTransactionsByPeriodAndPossiblyAccountIdAsync(TimeBasedAcquisitionBusinessModel model, LeadIdentityInfo leadInfo);
        Task<decimal> GetLeadBalanceAsync(int leadId, LeadIdentityInfo leadInfo);
    }
}