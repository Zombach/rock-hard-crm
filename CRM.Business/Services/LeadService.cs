using CRM.Business.IdentityInfo;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using MailExchange;
using MassTransit;
using System.Collections.Generic;
using CRM.Business.Constants;
using Google.Authenticator;


namespace CRM.Business.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILeadValidationHelper _leadValidationHelper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;

        public LeadService
        (
            ILeadRepository leadRepository,
            IAccountRepository accountRepository,
            IAuthenticationService authenticationService,
            ILeadValidationHelper leadValidationHelper,
            IPublishEndpoint publishEndpoint,
            ITwoFactorAuthenticatorService twoFactorAuthService

        )
        {
            _leadRepository = leadRepository;
            _accountRepository = accountRepository;
            _authenticationService = authenticationService;
            _leadValidationHelper = leadValidationHelper;
            _publishEndpoint = publishEndpoint;
            _twoFactorAuthService = twoFactorAuthService;
        }

        public LeadDto AddLead(LeadDto dto)
        {
            var tfaModel = _twoFactorAuthService.GetTwoFactorAuthenticatorKey();
            SetupCode setupInfo = tfaModel.Tfa.GenerateSetupCode("CRM", dto.Email, tfaModel.Key, false, 3);

            var qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            //string manualEntrySetupCode = setupInfo.ManualEntryKey;
            var qrCodeforEmail = $"<html><body><p>This is an HTML email with an embedded image.</p><p><img src={qrCodeImageUrl}/></p></body></html>";
            dto.Password = _authenticationService.HashPassword(dto.Password);
            dto.Role = Role.Regular;
            dto.BirthYear = dto.BirthDate.Year;
            dto.BirthMonth = dto.BirthDate.Month;
            dto.BirthDay = dto.BirthDate.Day;
            dto.KeyForTwoFactorAuth = tfaModel.Key;
            var leadId = _leadRepository.AddLead(dto);
            _accountRepository.AddAccount(new AccountDto { LeadId = leadId, Currency = Currency.RUB });
            EmailSender(dto, EmailMessages.RegistrationSubject, EmailMessages.RegistrationBody+$"{qrCodeforEmail}", true);

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
            var dto = _leadValidationHelper.GetLeadByIdAndThrowIfNotFound(leadId);
            EmailSender(dto, EmailMessages.DeleteLeadSubject, EmailMessages.DeleteLeadBody, false);
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

        private void EmailSender(LeadDto dto, string subject, string body, bool isBodyHtml)
        {
            _publishEndpoint.Publish<IMailExchangeModel>(new
            {
                Subject = subject,
                Body = $"{dto.LastName} {dto.FirstName} {body}",
                DisplayName = "Best CRM",
                MailAddresses = $"{dto.Email}",
                IsBodyHtml= isBodyHtml
            });
        }
    }
}