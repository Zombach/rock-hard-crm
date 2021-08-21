using CRM.Business.Options;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace CRM.Business.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;

        public LeadService(ILeadRepository leadRepository, IAccountRepository accountRepository)
        {
            _leadRepository = leadRepository;
            _accountRepository = accountRepository;
        }

        public LeadDto AddLead(LeadDto dto)
        {
            dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password, AuthOptions.WorkFactor, true);
            dto.Id = _leadRepository.AddLead(dto);
            return dto;
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

        public LeadDto GetLeadByEmail(string email)
        {
            var lead = _leadRepository.GetLeadByEmail(email);
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

        public List<LeadDto> GetLeadsByFilters(LeadFilterModel filters)
        {
            var leads = _leadRepository.GetAllLeads();
            if(!string.IsNullOrEmpty(filters.FirstName))
            {
                leads = leads.Where(l => l.FirstName.StartsWith(filters.FirstName)).ToList();
            }
            if(!string.IsNullOrEmpty(filters.LastName))
            {
                leads = leads.Where(l => l.LastName.StartsWith(filters.LastName)).ToList();
            }
            if(!string.IsNullOrEmpty(filters.Patronymic))
            {
                leads = leads.Where(l => l.Patronymic.StartsWith(filters.Patronymic)).ToList();
            }
            if(!(filters.Roles is null))
            {
                foreach (var role in filters.Roles)
                {
                    leads = leads.Where(l => l.Role == role).ToList();
                }
            }
            if (!(filters.Cities is null))
            {
                foreach (var city in filters.Cities)
                {
                    leads = leads.Where(l => l.City == city).ToList();
                }
            }
            if(!(filters.BirthDateFrom is null))
            {
                leads = leads.Where(l => l.BirthDate.CompareTo(filters.BirthDateFrom) < 0).ToList();
            }
            if (!(filters.BirthDateTo is null))
            {
                leads = leads.Where(l => l.BirthDate.CompareTo(filters.BirthDateTo) > 0).ToList();
            }
            return leads;
        }
    }
}