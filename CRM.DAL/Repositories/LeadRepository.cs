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
        private const string _updateLeadRoleProcedure = "dbo.Lead_UpdateRole";

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
                    lead.BirthDate,
                    lead.BirthYear,
                    lead.BirthMonth,
                    lead.BirthDay
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

        public void UpdateLeadRole(LeadDto lead)
        {
            _connection.Execute(
                _updateLeadRoleProcedure,
                new
                {
                    lead.Id,
                    lead.Role
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public int DeleteLead(int id)
        {
            return _connection
                .Execute(
                    _deleteLeadByIdProcedure,
                    new { id },
                    commandType: CommandType.StoredProcedure);
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

        public List<LeadDto> GetAllLeads()
        {
            var leadDictionary = new Dictionary<int, LeadDto>();

            return _connection
                .Query<LeadDto, AccountDto, CityDto, Role, LeadDto>(
                    _getAllLeadsProcedure,
                    (lead, account, city, role) =>
                    {

                        if (!leadDictionary.TryGetValue(lead.Id, out var leadEntry))
                        {
                            leadEntry = lead;
                            leadEntry.City = city;
                            leadEntry.Role = role;
                            leadEntry.Accounts = new List<AccountDto>();
                            leadDictionary.Add(lead.Id, leadEntry);
                        }
                        leadEntry.Accounts.Add(account);

                        return leadEntry;
                    },
                    splitOn: "id",
                    commandType: CommandType.StoredProcedure)
                .Distinct()
                .ToList();
        }

        public List<LeadDto> GetLeadsByFilters(SqlResult sqlResult)
        {
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
    }
}