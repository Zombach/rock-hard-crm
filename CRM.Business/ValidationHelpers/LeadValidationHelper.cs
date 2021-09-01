using CRM.Business.Constants;
using CRM.Business.Exceptions;
using CRM.DAL.Models;
using CRM.DAL.Repositories;

namespace DevEdu.Business.ValidationHelpers
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
    }
}