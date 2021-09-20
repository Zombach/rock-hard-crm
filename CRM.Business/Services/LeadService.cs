using CRM.Business.IdentityInfo;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using MailExchange;
using MassTransit;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        public async Task<LeadDto> AddLeadAsync(LeadDto dto)
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
            var leadId = await _leadRepository.AddLeadAsync(dto);
            await EmailSender(dto, EmailMessages.RegistrationSubject, EmailMessages.RegistrationBody+$"{qrCodeforEmail}", true);
            await _accountRepository.AddAccountAsync(new AccountDto { LeadId = leadId, Currency = Currency.RUB });

            return await _leadRepository.GetLeadByIdAsync(leadId);
        }

        public async Task<LeadDto> UpdateLeadAsync(int leadId, LeadDto dto)
        {
            await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
            dto.Id = leadId;
            await _leadRepository.UpdateLeadAsync(dto);
            return await _leadRepository.GetLeadByIdAsync(leadId);
        }

        public async Task<LeadDto> UpdateLeadRoleAsync(int leadId, Role role)
        {
            var dto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
            dto.Role = role;
            await _leadRepository.UpdateLeadRoleAsync(dto);
            return await _leadRepository.GetLeadByIdAsync(leadId);
        }

        public async Task DeleteLeadAsync(int leadId)
        {
            var dto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
            await EmailSender(dto, EmailMessages.DeleteLeadSubject, EmailMessages.DeleteLeadBody, false);
            await _leadRepository.DeleteLeadAsync(leadId);
        }

        public async Task<LeadDto> GetLeadByIdAsync(int leadId, LeadIdentityInfo leadInfo)
        {
            var dto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
            _leadValidationHelper.CheckAccessToLead(leadId, leadInfo);
            return dto;
        }

        public async Task<List<LeadDto>> GetAllLeadsAsync()
        {
            return await _leadRepository.GetAllLeadsAsync();
        }

        private async Task EmailSender(LeadDto dto, string subject, string body, bool isBodyHtml)
        {
            await _publishEndpoint.Publish<IMailExchangeModel>(new
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