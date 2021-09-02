using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Dapper;
using DapperQueryBuilder;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using SqlKata;
using SqlKata.Compilers;
using System;

namespace CRM.DAL.Repositories
{
    public class LeadRepository : BaseRepository, ILeadRepository
    {
        private const string _insertLeadProcedure = "dbo.Lead_Insert";
        private const string _updateLeadProcedure = "dbo.Lead_Update";
        private const string _getLeadByIdProcedure = "dbo.Lead_SelectById";
        private const string _deleteLeadByIdProcedure = "dbo.Lead_Delete";
        private const string _getLeadByEmailProcedure = "dbo.Lead_SelectByEmail";
        private const string _getAllLeadsProcedure = "dbo.Lead_SelectAll";
        private const string _cities = "dbo.Cities";

        public LeadRepository(IOptions<DatabaseSettings> options) : base(options) { }

        public int AddLead(LeadDto lead)
        {
            return _connection.QuerySingleOrDefault<int>(
                _insertLeadProcedure,
                new
                {
                    lead.FirstName,
                    lead.LastName,
                    lead.Patronymic,
                    lead.Email,
                    lead.PhoneNumber,
                    lead.Password,
                    role = (int)lead.Role,
                    cityId = lead.City.Id,
                    lead.BirthDate
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void UpdateLead(LeadDto lead)
        {
            _connection.Execute(
                _updateLeadProcedure,
                new
                {
                    lead.Id,
                    lead.FirstName,
                    lead.LastName,
                    lead.Patronymic,
                    lead.Email,
                    lead.PhoneNumber,
                    lead.BirthDate
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public List<LeadDto> GetAllLeads()
        {
            //var leadDictionary = new Dictionary<int, LeadDto>();
            return _connection
                .Query<LeadDto, CityDto, Role, LeadDto>(
                _getAllLeadsProcedure,
                (lead, city, role) =>
                {
                    lead.Role = role;
                    lead.City = city;
                    return lead;
                },
                commandType: CommandType.StoredProcedure)
                .ToList();
        }

        public LeadDto GetLeadById(int id)
        {
            LeadDto result = default;
            return _connection
                .Query<LeadDto, AccountDto, CityDto, Role, LeadDto>(
                _getLeadByIdProcedure,
                (lead, account, city, role) =>
                {
                    if (result == null)
                    {
                        result = lead;
                        result.City = city;
                        result.Role = role;
                        result.Accounts = new List<AccountDto>();
                    }
                    result.Accounts.Add(account);
                    return result;
                },
                new { id },
                splitOn: "id",
                commandType: CommandType.StoredProcedure)
                .FirstOrDefault();
        }

        public LeadDto GetLeadByEmail(string email)
        {
            LeadDto result = default;
            return _connection
                .Query<LeadDto, AccountDto, CityDto, Role, LeadDto>(
                _getLeadByEmailProcedure,
                (lead, account, city, role) =>
                {
                    if (result == null)
                    {
                        result = lead;
                        result.City = city;
                        result.Role = role;
                        result.Accounts = new List<AccountDto>();
                    }
                    result.Accounts.Add(account);
                    return result;
                },
                new { email },
                commandType: CommandType.StoredProcedure)
                .FirstOrDefault();
        }

        public int DeleteLeadById(int id)
        {
            return _connection
                .Execute(
                _deleteLeadByIdProcedure,
                new { id },
                commandType: CommandType.StoredProcedure);
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
                    BuildStringForCompare(filter.SearchType),
                    CreateStringBySearchingType(filter.SearchType, filter.FirstName));
            }
            if (!String.IsNullOrEmpty(filter.LastName))
            {
                query = query.Where(
                    "l.LastName",
                    BuildStringForCompare(filter.SearchType),
                    CreateStringBySearchingType(filter.SearchType, filter.LastName));
            }
            if (!String.IsNullOrEmpty(filter.Patronymic))
            {
                query = query.Where(
                    "l.Patronymic",
                    BuildStringForCompare(filter.SearchType),
                    CreateStringBySearchingType(filter.SearchType, filter.Patronymic));
            }
            if (filter.Role != null)
            {
                query = query.Where("l.Role", filter.Role); 
            }
            if (filter.City != null || filter.City.Count != 0)
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

            var result = _connection
                .Query<LeadDto, CityDto, Role, LeadDto>(
                sqlResult.Sql,
                (lead, city, role) =>
                {
                    lead.Role = role;
                    lead.City = city;
                    return lead;
                },
                param: sqlResult.NamedBindings,
                splitOn: "id",
                commandType: CommandType.Text)
                .ToList();
            return result;
        }

        private string CreateStringBySearchingType(SearchType searchType, string searchingString)
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
                case SearchType.Equals:
                    break;
                case SearchType.NotEquals:
                    searchingString = "<>" + searchingString; 
                    break;
            }
            return searchingString;
        }

        private string BuildStringForCompare(SearchType searchType)
        {
            string like = "LIKE";
            if(searchType == SearchType.NotEquals)
            {
                like = "NOT " + like;
            }
            return like;
        }
    }
}