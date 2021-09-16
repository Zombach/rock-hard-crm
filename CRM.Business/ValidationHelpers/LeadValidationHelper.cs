using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.Business.IdentityInfo;
using CRM.DAL.Models;
using CRM.DAL.Repositories;

namespace CRM.Business.ValidationHelpers
{
    public class LeadValidationHelper : ILeadValidationHelper
    {
        private readonly ILeadRepository _leadRepository;

        public LeadValidationHelper(ILeadRepository leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public LeadDto GetLeadByIdAndThrowIfNotFound(int leadId)
        {
            var lead = _leadRepository.GetLeadById(leadId);
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