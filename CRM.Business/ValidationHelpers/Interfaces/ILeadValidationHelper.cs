﻿using CRM.Business.IdentityInfo;
using CRM.DAL.Models;
using System.Threading.Tasks;

namespace CRM.Business.ValidationHelpers
{
    public interface ILeadValidationHelper
    {
        Task<LeadDto> GetLeadByIdAndThrowIfNotFoundAsync(int leadId);
        void CheckAccessToLead(int leadId, LeadIdentityInfo leadInfo);
    }
}