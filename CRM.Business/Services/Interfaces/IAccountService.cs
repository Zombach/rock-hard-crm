using CRM.Business.IdentityInfo;
using CRM.Business.Models;
using CRM.DAL.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.Business.Services
{
    public interface IAccountService
    {
        Task<int> AddAccountAsync(Currency currency, LeadIdentityInfo leadInfo);
        Task DeleteAccountAsync(int accountId, int leadId);
        Task RestoreAccountAsync(int accountId, int leadId);
        Task<AccountBusinessModel> GetAccountWithTransactionsAsync(int accountId, LeadIdentityInfo leadInfo);
        Task<List<AccountBusinessModel>> GetTransactionsByPeriodAndPossiblyAccountIdAsync(TimeBasedAcquisitionBusinessModel timeBasedModel, LeadIdentityInfo leadInfo);
        Task<List<TransactionBusinessModel>> GetTransactionsByAccountIdsForTwoMonthsAsync(List<int> accountIds, LeadIdentityInfo leadInfo);
        Task<decimal> GetLeadBalanceAsync(int leadId, LeadIdentityInfo leadInfo);
    }
}