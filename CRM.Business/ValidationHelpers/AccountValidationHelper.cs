using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Linq;

namespace CRM.Business.ValidationHelpers
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
                throw new AuthorizationException(string.Format(ServiceMessages.LeadHasNoAccessMessageToAccount, leadId));
        }

        public void CheckForVipAccess(Currency currency, LeadIdentityInfo leadInfo)
        {
            if (currency is not (Currency.RUB or Currency.USD) && !leadInfo.IsVip())
                throw new AuthorizationException(string.Format(ServiceMessages.LeadHasNoAccessMessageByRole, leadInfo.LeadId));
        }

        public void CheckForDuplicateCurrencies(LeadDto lead, Currency currency)
        {
            //нужна ли проверка на аккаунт нал?
            var account = lead.Accounts.FirstOrDefault(ac => ac.Currency == currency);
            if (account != default)
                throw new ValidationException(nameof(account), string.Format(ServiceMessages.LeadHasThisCurrencyMessage, lead.Id));
        }
    }
}