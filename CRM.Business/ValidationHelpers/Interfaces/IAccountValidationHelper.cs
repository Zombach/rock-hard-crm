using CRM.DAL.Enums;
using CRM.DAL.Models;

namespace DevEdu.Business.ValidationHelpers
{
    public interface IAccountValidationHelper
    {
        AccountDto GetAccountByIdAndThrowIfNotFound(int accountId);
        void CheckLeadAccessToAccount(int verifiableId, int leadId);
        void CheckForDuplicateCurrencies(LeadDto lead, Currency currency);
    }
}