using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;

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
            var id = _leadRepository.AddLead(dto);
            return _leadRepository.GetLeadById(id);
        }

        public LeadDto UpdateLead(int id, LeadDto dto)
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
    }
}