using CRM.DAL.Enums;
using CRM.DAL.Models;
using CRM.DAL.Repositories;
using System.Collections.Generic;
using DevEdu.Business.ValidationHelpers;
using SqlKata.Compilers;
using SqlKata;
using System;

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
                                                 "City.Id",
                                                 "City.Name",
                                                 "l.Role as Id",
                                                 "l.RegistrationDate");

            if (!String.IsNullOrEmpty(filter.FirstName))
            {
                query = query.Where(
                    "l.FirstName",
                    BuildStringForCompare(filter.SearchTypeForFirstName),
                    CreateStringBySearchingType(filter.SearchTypeForFirstName, filter.FirstName));
            }
            if (!String.IsNullOrEmpty(filter.LastName))
            {
                query = query.Where(
                    "l.LastName",
                    BuildStringForCompare(filter.SearchTypeForLastName),
                    CreateStringBySearchingType(filter.SearchTypeForLastName, filter.LastName));
            }
            if (!String.IsNullOrEmpty(filter.Patronymic))
            {
                query = query.Where(
                    "l.Patronymic",
                    BuildStringForCompare(filter.SearchTypeForPatronymic),
                    CreateStringBySearchingType(filter.SearchTypeForPatronymic, filter.Patronymic));
            }
            if (filter.Role != null)
            {
                query = query.Where("l.Role", filter.Role);
            }
            if (filter.City != null && filter.City.Count != 0)
            {
                query = query.WhereIn("City.Id", filter.City);
            }
            if (filter.BirthDateFrom != null)
            {
                query = query.WhereDate("l.BirthDate", ">", filter.BirthDateFrom.ToString());
            }
            if (filter.BirthDateTo != null)
            {
                query = query.WhereDate("l.BirthDate", "<", filter.BirthDateTo.ToString());
            }

            query = query
                .Where("l.IsDeleted", 0)
                .Join("City", "City.Id", "l.CityId");

            SqlResult sqlResult = compiler.Compile(query);

            var result = _leadRepository.GetLeadsByFilters(sqlResult);
            return result;
        }

        private string CreateStringBySearchingType(SearchType? searchType, string searchingString)
        {
            switch (searchType)
            {
                case SearchType.StartsWith:
                    searchingString = searchingString + "%";
                    break;
                case SearchType.Contains:
                    searchingString = "%" + searchingString + "%";
                    break;
                case SearchType.EndsWith:
                    searchingString = "%" + searchingString;
                    break;
                default:
                    return searchingString;
            }
            return searchingString;
        }

        private string BuildStringForCompare(SearchType? searchType)
        {
            string like = "=";
            if (searchType == SearchType.NotEquals)
            {
                like = "!" + like;
            }
            return like;
        }
    }
}