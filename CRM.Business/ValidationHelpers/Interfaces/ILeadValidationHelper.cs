using CRM.Business.IdentityInfo;
using CRM.DAL.Models;

namespace DevEdu.Business.ValidationHelpers
{
    public interface ILeadValidationHelper
    {
        LeadDto GetLeadByIdAndThrowIfNotFound(int leadId);
        void CheckAccessToLead(int leadId, LeadIdentityInfo leadInfo);
    }
}