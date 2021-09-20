using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using System.Threading.Tasks;

namespace CRM.Business.ValidationHelpers
{
    public interface IAccountValidationHelper
    {
        Task<AccountDto> GetAccountByIdAndThrowIfNotFoundAsync(int accountId);
        void CheckLeadAccessToAccount(int verifiableId, int leadId);
        void CheckForDuplicateCurrencies(LeadDto lead, Currency currency);
        void CheckForVipAccess(Currency currency, LeadIdentityInfo leadInfo);
    }
}