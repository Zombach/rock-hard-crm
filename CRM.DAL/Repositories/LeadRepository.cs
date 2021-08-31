using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Dapper;
using DapperQueryBuilder;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        private const string _getAllLeadsByFilters = "dbo.Lead_SelectAllByFilters";

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
            var query = _connection.QueryBuilder
                            (@$"SELECT 
		                    l.Id,
		                    l.FirstName,
		                    l.LastName,
		                    l.Patronymic,
		                    l.Email,
		                    c.Id,
		                    c.Name,
		                    l.Role as Id
	                        FROM dbo.[Lead] l
	                        INNER JOIN City c on c.Id = l.CityId 
                            WHERE IsDeleted = 0 ");

            if (filter.SearchType == SearchType.StartsWith)
            {
                query.AppendLine($"AND l.FirstName LIKE {filter.FirstName}%");
                query.AppendLine($"AND l.LastName LIKE {filter.LastName}%");
                query.AppendLine($"AND l.Patronymic LIKE {filter.Patronymic}%");
                //query.Where($"FirstName LIKE {firstName}");
                //query.Where($"LastName LIKE {lastName}");
                //query.Where($"Patronymic LIKE '{patronymic}'");
            }

            var result =  _connection
                .Query<LeadDto, CityDto, Role, LeadDto>(
                query.Sql,
                (lead, city, role) =>
                {
                    lead.Role = role;
                    lead.City = city;
                    return lead;
                },
                new
                {
                    filter.FirstName,
                    filter.LastName,
                    filter.Patronymic
                },
                commandType: CommandType.Text,
                splitOn: "Id")
                .ToList();
            return result;
        }
    }
}