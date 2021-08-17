using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;

namespace CRM.Business.Services
{
    public class LeadService : ILeadService
    {
        private ILeadRepository _leadRepository;

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
            return _leadRepository.GetLeadById(id);
        }

        public List<LeadDto> GetAllLeads() => _leadRepository.GetAllLeads();

        public LeadDto GetLeadById(int id)
        {
            return _leadRepository.GetLeadById(id);
        }

        public LeadDto GetLeadById(string email)
        {
            return _leadRepository.GetLeadByEmail(email);
        }

        public void DeleteLeadById(int id)
        {
            _leadRepository.DeleteLeadById(id);
        }

    }
}
