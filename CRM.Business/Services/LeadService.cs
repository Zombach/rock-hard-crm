using CRM.Business.FilterModels;
using CRM.Business.Options;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System;
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
            var leads = _leadRepository.GetAllLeads().AsQueryable();
            if (!string.IsNullOrEmpty(filters.FirstName))
            {
                leads = leads.Where(l => IsSearchForString(filters.SearchType, l.FirstName, filters.FirstName));
            }
            if(!string.IsNullOrEmpty(filters.LastName))
            {
                leads = leads.Where(l => IsSearchForString(filters.SearchType, l.LastName, filters.LastName));
            }
            if(!string.IsNullOrEmpty(filters.Patronymic))
            {
                leads = leads.Where(l => IsSearchForString(filters.SearchType, l.LastName, filters.LastName));
            }
            if(!(filters.Roles is null || filters.Roles.Count == 0))
            {
                var filtered = new List<LeadDto>();
                foreach (var role in filters.Roles)
                {
                    filtered.AddRange(leads.Where(l => l.Role.Equals(role)));
                }
                leads = filtered.AsQueryable();
            }
            if (!(filters.Cities is null || filters.Cities.Count == 0))
            {
                var filtered = new List<LeadDto>();
                foreach (var city in filters.Cities)
                {
                    filtered.AddRange(leads.Where(l => l.City.Equals(city)));
                }
                leads = filtered.AsQueryable();
            }
            if(!(filters.BirthDateFrom is null))
            {
                leads = leads.Where(l => l.BirthDate.CompareTo(filters.BirthDateFrom) > 0);
            }
            if (!(filters.BirthDateTo is null))
            {
                leads = leads.Where(l => l.BirthDate.CompareTo(filters.BirthDateTo) < 0);
            }
            return leads.ToList();
        }

        private bool IsSearchForString(SearchType searchType, string a, string b)
        {
            switch (searchType)
            {
                case (SearchType.StartsWith):
                    return a.StartsWith(b);
                case (SearchType.Contains):
                    return a.Contains(b);                    
            }
            return false;
        }
    }
}