using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;
using CRM.Business.IdentityInfo;

namespace CRM.Business.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthenticationService _authenticationService;

        public LeadService(ILeadRepository leadRepository, IAccountRepository accountRepository, IAuthenticationService authenticationService)
        {
            _leadRepository = leadRepository;
            _accountRepository = accountRepository;
            _authenticationService = authenticationService;
        }

        public LeadDto AddLead(LeadDto dto)
        {
            dto.Password = _authenticationService.HashPassword(dto.Password);
            dto.Role = Role.Regular;
            var id = _leadRepository.AddLead(dto);
            _accountRepository.AddAccount(new AccountDto { LeadId = id, Currency = Currency.RUB });
            return _leadRepository.GetLeadById(id);
        }

        public LeadDto UpdateLead(int id, LeadDto dto, UserIdentityInfo userIdentityInfo)
        {
            dto.Id = id;
            _leadRepository.UpdateLead(dto);
            var lead = _leadRepository.GetLeadById(id);
            return lead;
        }

        public List<LeadDto> GetAllLeads()
        {
            var list = _leadRepository.GetAllLeads();
            return list;
        }

        public LeadDto GetLeadById(int id)
        {
            var lead = _leadRepository.GetLeadById(id);
            return lead;
        }

        public void DeleteLeadById(int id)
        {
            _leadRepository.DeleteLeadById(id);
        }

        public int AddAccount(AccountDto dto)
        {
            var accountId = _accountRepository.AddAccount(dto);
            return accountId;
        }

        public void DeleteAccount(int id)
        {
            _accountRepository.DeleteAccount(id);
        }

        public List<LeadDto> GetLeadsByFilters(LeadFiltersDto filter)
        {
            var result = _leadRepository.GetLeadsByFilters(filter);
            return result;
        }
    }
}