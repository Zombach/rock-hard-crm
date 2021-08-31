using CRM.DAL.Models;

namespace DevEdu.Business.ValidationHelpers
{
    public interface ILeadValidationHelper
    {
        LeadDto GetLeadByIdAndThrowIfNotFound(int leadId);
    }
}