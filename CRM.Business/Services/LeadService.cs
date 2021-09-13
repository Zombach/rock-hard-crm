using CRM.Business.IdentityInfo;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Enums;
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
        private readonly ILeadValidationHelper _leadValidationHelper;

        public LeadService
        (
            ILeadRepository leadRepository,
            IAccountRepository accountRepository,
            IAuthenticationService authenticationService,
            ILeadValidationHelper leadValidationHelper
        )
        {
            _leadRepository = leadRepository;
            _accountRepository = accountRepository;
            _authenticationService = authenticationService;
            _leadValidationHelper = leadValidationHelper;
        }

        public LeadDto AddLead(LeadDto dto)
        {
            dto.Password = _authenticationService.HashPassword(dto.Password);
            dto.Role = Role.Regular;
            dto.BirthYear = dto.BirthDate.Year;
            dto.BirthMonth = dto.BirthDate.Month;
            dto.BirthDay = dto.BirthDate.Day;
            var leadId = _leadRepository.AddLead(dto);

            _accountRepository.AddAccount(new AccountDto { LeadId = leadId, Currency = Currency.RUB });
            return _leadRepository.GetLeadById(leadId);
        }

        public LeadDto UpdateLead(int leadId, LeadDto dto)
        {
            _leadValidationHelper.GetLeadByIdAndThrowIfNotFound(leadId);
            dto.Id = leadId;
            _leadRepository.UpdateLead(dto);
            return _leadRepository.GetLeadById(leadId);
        }

        public LeadDto UpdateLeadRole(int leadId, Role role)
        {
            var dto = _leadValidationHelper.GetLeadByIdAndThrowIfNotFound(leadId);
            dto.Role = role;
            _leadRepository.UpdateLeadRole(dto);
            return _leadRepository.GetLeadById(leadId);
        }

        public void DeleteLead(int leadId)
        {
            _leadValidationHelper.GetLeadByIdAndThrowIfNotFound(leadId);
            _leadRepository.DeleteLead(leadId);
        }

        public LeadDto GetLeadById(int leadId, LeadIdentityInfo leadInfo)
        {
            var dto = _leadValidationHelper.GetLeadByIdAndThrowIfNotFound(leadId);
            _leadValidationHelper.CheckAccessToLead(leadId, leadInfo);
            return dto;
        }

        public List<LeadDto> GetAllLeads()
        {
            return _leadRepository.GetAllLeads();
        }
    }
}