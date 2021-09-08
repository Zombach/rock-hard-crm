using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using DevEdu.Business.ValidationHelpers;
using SqlKata.Compilers;
using SqlKata;
using System;
using CRM.Business.Extensions;

namespace CRM.Business.Services
{
    public class LeadService : ILeadService
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IAuthenticationService _authenticationService;
        private readonly ILeadValidationHelper _leadValidationHelper;

        public LeadService
        (
            ILeadRepository leadRepository,
            IAccountRepository accountRepository,
            IAuthenticationService authenticationService,
            ILeadValidationHelper leadValidationHelper
        )
        {
            _leadRepository = leadRepository;
            _accountRepository = accountRepository;
            _authenticationService = authenticationService;
            _leadValidationHelper = leadValidationHelper;
        }

        public LeadDto AddLead(LeadDto dto)
        {
            dto.Password = _authenticationService.HashPassword(dto.Password);
            dto.Role = Role.Regular;
            var id = _leadRepository.AddLead(dto);
            _accountRepository.AddAccount(new AccountDto { LeadId = id, Currency = Currency.RUB });
            return _leadRepository.GetLeadById(id);
        }

        public LeadDto UpdateLead(int id, LeadDto dto)
        {
            _leadValidationHelper.GetLeadByIdAndThrowIfNotFound(id);
            dto.Id = id;
            _leadRepository.UpdateLead(dto);
            return _leadRepository.GetLeadById(id);
        }

        public List<LeadDto> GetAllLeads()
        {
            return _leadRepository.GetAllLeads();
        }

        public LeadDto GetLeadById(int id)
        {
            return _leadValidationHelper.GetLeadByIdAndThrowIfNotFound(id);
        }

        public void DeleteLeadById(int id)
        {
            _leadValidationHelper.GetLeadByIdAndThrowIfNotFound(id);
            _leadRepository.DeleteLeadById(id);
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
                                                 "l.RegistrationDate",
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
    }
}