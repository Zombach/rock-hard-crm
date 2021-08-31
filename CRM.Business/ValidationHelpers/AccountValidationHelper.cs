using System.Linq;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;

namespace DevEdu.Business.ValidationHelpers
{
    public class AccountValidationHelper : IAccountValidationHelper
    {
        private readonly IAccountRepository _accountRepository;

        public AccountValidationHelper(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public AccountDto GetAccountByIdAndThrowIfNotFound(int accountId)
        {
            var account = _accountRepository.GetAccountById(accountId);
            if (account == default)
                throw new EntityNotFoundException(string.Format(ServiceMessages.EntityNotFoundMessage, nameof(account), accountId));
            return account;
        }

        public void CheckLeadAccessToAccount(int verifiableId, int leadId)
        {
            if (verifiableId != leadId)
                throw new AuthorizationException(string.Format(ServiceMessages.LeadHasNoAccessMessage, leadId));
        }

        public void CheckForDuplicateCurrencies(LeadDto lead, Currency currency)
        {
            var account = lead.Accounts.FirstOrDefault(ac=>ac.Currency==currency);
            if (account != default)
                throw new ValidationException(nameof(account),string.Format(ServiceMessages.LeadHasThisCurrencyMessage, lead.Id));
        }
    }
}