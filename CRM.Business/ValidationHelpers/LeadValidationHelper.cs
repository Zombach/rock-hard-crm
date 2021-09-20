using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Threading.Tasks;

namespace CRM.Business.ValidationHelpers
{
    public class LeadValidationHelper : ILeadValidationHelper
    {
        private readonly ILeadRepository _leadRepository;

        public LeadValidationHelper(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public async Task<LeadDto> GetLeadByIdAndThrowIfNotFoundAsync(int leadId)
        {
            var lead = await _leadRepository.GetLeadByIdAsync(leadId);
            if (lead == default)
                throw new EntityNotFoundException(string.Format(ServiceMessages.EntityNotFoundMessage, nameof(lead), leadId));
            return lead;
        }

        public void CheckAccessToLead(int leadId, LeadIdentityInfo leadInfo)
        {
            if (leadInfo.IsAdmin()) return;
            if (leadInfo.LeadId != leadId)
                throw new EntityNotFoundException(string.Format(ServiceMessages.LeadHasNoAccessMessageToLead, leadInfo.LeadId));
        }
    }
}