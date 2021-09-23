using CRM.Business.Constants;
using CRM.Business.Extensions;
using CRM.Business.IdentityInfo;
using CRM.Business.ValidationHelpers;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using SqlKata;
using SqlKata.Compilers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Authenticator;
using MassTransit;

namespace CRM.Business.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly IEmailSenderService _emailSenderService;
        private readonly ILeadValidationHelper _leadValidationHelper;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ITwoFactorAuthenticatorService _twoFactorAuthService;

        public LeadService
        (
            IEmailSenderService emailSenderService,
            ILeadRepository leadRepository,
            IAccountRepository accountRepository,
            IAuthenticationService authenticationService,
            ILeadValidationHelper leadValidationHelper,
            IPublishEndpoint publishEndpoint,
            ITwoFactorAuthenticatorService twoFactorAuthService

        )
        {
            _emailSenderService = emailSenderService;
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
            SetupCode setupInfo = tfaModel.Tfa.GenerateSetupCode("CRM", dto.Email, tfaModel.Key, false, 6);
            var qrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;


            dto.Password = _authenticationService.HashPassword(dto.Password);
            dto.Role = Role.Regular;
            dto.BirthYear = dto.BirthDate.Year;
            dto.BirthMonth = dto.BirthDate.Month;
            dto.BirthDay = dto.BirthDate.Day;
            var leadId = await _leadRepository.AddLeadAsync(dto);
            var keyId = AddTwoFactorKeyToLeadAsync(leadId, tfaModel.Key);          
            await _accountRepository.AddAccountAsync(new AccountDto { LeadId = leadId, Currency = Currency.RUB });
            await _emailSenderService.EmailSenderAsync(dto, EmailMessages.RegistrationSubject, EmailMessages.RegistrationBody, string.Format(EmailMessages.QRCode, qrCodeImageUrl));

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

        public async Task UpdateLeadRoleBulkAsync(List<LeadDto> leads)
        {
            await _leadRepository.UpdateLeadRoleBulkAsync(leads);
        }

        public async Task DeleteLeadAsync(int leadId)
        {
            var dto = await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
            await _emailSenderService.EmailSenderAsync(dto, EmailMessages.DeleteLeadSubject, EmailMessages.DeleteLeadBody);
            await _leadRepository.DeleteLeadAsync(leadId);
        }

        public async Task<LeadDto> GetLeadByIdAsync(int leadId, LeadIdentityInfo leadInfo)
        {
            if (!leadInfo.IsAdmin())
                _leadValidationHelper.CheckAccessToLead(leadId, leadInfo);

            return await _leadValidationHelper.GetLeadByIdAndThrowIfNotFoundAsync(leadId);
        }

        public List<LeadDto> GetLeadsByFilters(LeadFiltersDto filter)
        {
            var compiler = new SqlServerCompiler();

            var query = new Query("Lead as l").Select("l.Id",
                                                 "l.FirstName",
                                                 "l.LastName",
                                                 "l.Patronymic",
                                                 "l.Email",
                                                 "l.BirthDate",
                                                 "City.Id",
                                                 "City.Name",
                                                 "l.Role as Id");

            query = this.FilterByName(query, filter.FirstName, filter.SearchTypeForFirstName, "FirstName");
            query = this.FilterByName(query, filter.LastName, filter.SearchTypeForLastName, "LastName");
            query = this.FilterByName(query, filter.Patronymic, filter.SearchTypeForPatronymic, "Patronymic");
            query = this.FilterByRole(query, filter);
            query = this.FilterByCity(query, filter);
            query = this.FilterByBirthDate(query, filter);

            query = query
                .Where("l.IsDeleted", 0)
                .Join("City", "City.Id", "l.CityId");

            SqlResult sqlResult = compiler.Compile(query);

            return _leadRepository.GetLeadsByFilters(sqlResult);
        }

        public async Task<List<LeadDto>> GetAllLeadsAsync()
        {
            return await _leadRepository.GetAllLeadsAsync();
        }

        public List<LeadDto> GetAllLeadsByBatches(int lastLeadId)
        {
            return _leadRepository.GetAllLeadsByBatches(lastLeadId);
        }

        public async Task<int>AddTwoFactorKeyToLeadAsync(int leadId, string key)
        {
            return await _leadRepository.AddTwoFactorKeyToLeadAsync(leadId, key);
        }

        public async Task<string> GetTwoFactorKeyAsync(int leadId)
        {
            return await _leadRepository.GetTwoFactorKeyAsync(leadId);
        }
    }
}