using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;

        public LeadService(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public LeadDto AddLead(LeadDto dto)
        {
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

        public LeadDto GetLeadById(string email)
        {
            var lead = _leadRepository.GetLeadByEmail(email);
            return lead;
        }

        public void DeleteLeadById(int id)
        {
            _leadRepository.DeleteLeadById(id);
        }
    }
}