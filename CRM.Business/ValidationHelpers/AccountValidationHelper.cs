using System;
using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using CRM.Business.Models;

namespace CRM.Business.ValidationHelpers
{
    public class AccountValidationHelper : IAccountValidationHelper
    {
        private readonly IAccountRepository _accountRepository;

        public AccountValidationHelper(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        public async Task<AccountDto> GetAccountByIdAndThrowIfNotFoundAsync(int accountId)
        {
            var account = await _accountRepository.GetAccountByIdAsync(accountId);
            if (account == default)
                throw new EntityNotFoundException(string.Format(ServiceMessages.EntityNotFoundMessage, nameof(account), accountId));
            
            return account;
        }

        public void CheckLeadAccessToAccount(int verifiableId, int leadId)
        {
            if (verifiableId != leadId)
                throw new AuthorizationException(string.Format(ServiceMessages.LeadHasNoAccessMessageToAccount, leadId, verifiableId));
        }

        public void CheckForVipAccess(Currency currency, LeadIdentityInfo leadInfo)
        {
            if (currency is not (Currency.RUB or Currency.USD) && !leadInfo.IsVip())
                throw new AuthorizationException(string.Format(ServiceMessages.LeadHasNoAccessMessageByRole, leadInfo.LeadId));
        }

        public void CheckForDuplicateCurrencies(LeadDto lead, Currency currency)
        {
            var account = lead.Accounts.FirstOrDefault(ac => ac.Currency == currency);
            if (account != default)
                throw new ValidationException(nameof(account), string.Format(ServiceMessages.LeadHasThisCurrencyMessage, lead.Id));
        }

        public void CheckForDuplicateTransaction(long transactionId, AccountBusinessModel account)
        {
            if (transactionId == 0)
            {
                throw new ValidationException(nameof(account),
                    string.Format(ServiceMessages.DuplicateTransactions, account.Id));
            }
        }

        public void CheckBalance(AccountBusinessModel account, decimal amount)
        {
            if (account.Balance - amount < 0)
            {
                throw new ValidationException(nameof(amount), string.Format(ServiceMessages.DoesNotHaveEnoughMoney, account.Id, account.Balance));
            }
        }

        public DateTime GetTransactionsLastDateAndThrowIfNotFound(AccountBusinessModel account)
        {
            if (account.Transfers.Count == 0 && account.Transactions.Count == 0)
                throw new ValidationException(nameof(account),
                    string.Format(ServiceMessages.DoesNotHaveTransactions, account.Id));

            if (account.Transfers.Count == 0)
                return account.Transactions.Last().Date;

            if (account.Transactions.Count == 0)
                return account.Transfers.Last().Date;

            var transferLastDate = account.Transfers.Last().Date;
            var transactionLastDate = account.Transactions.Last().Date;
            return transferLastDate > transactionLastDate ? transferLastDate : transactionLastDate;
        }
    }
}