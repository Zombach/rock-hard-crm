﻿using CRM.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRM.DAL.Repositories
{
    public interface IAccountRepository
    {
        Task<int> AddAccountAsync(AccountDto dto);
        Task DeleteAccountAsync(int id);
        Task RestoreAccountAsync(int id);
        Task<AccountDto> GetAccountByIdAsync(int id);
        Task<List<AccountDto>> GetAccountsByListIdAsync(List<int> accountsId);
    }
}