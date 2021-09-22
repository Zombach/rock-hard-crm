using CRM.Core;
using CRM.DAL.Enums;
using CRM.DAL.Models;
using Dapper;
using Microsoft.Extensions.Options;
using SqlKata;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

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
        private const string _getAllLeadsByBatchesProcedure = "dbo.Lead_SelectAllByBatchesWithoutAdmins";
        private const string _updateLeadRoleProcedure = "dbo.Lead_UpdateRole";
        private const string _updateListRoleLeadsProcedure = "dbo.LeadsList_Role_Update";
        private const string _LeadDtoType = "dbo.LeadDtoType";

        public LeadRepository(IOptions<DatabaseSettings> options) : base(options) { }

        public async Task<int> AddLeadAsync(LeadDto lead)
        {
            return await _connection.QuerySingleOrDefaultAsync<int>(
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

        public async Task UpdateLeadAsync(LeadDto lead)
        {
            await _connection.ExecuteAsync(
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

        public async Task UpdateLeadRoleAsync(LeadDto lead)
        {
            await _connection.ExecuteAsync(
                _updateLeadRoleProcedure,
                new
                {
                    lead.Id,
                    lead.Role
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void ChangeRoleForLeads(List<LeadDto> listLeadDtos)
        {
            var dt = new DataTable();
            dt.Columns.Add("LeadId");
            dt.Columns.Add("Role");

            foreach (var lead in listLeadDtos)
            {
                dt.Rows.Add(lead.Id, (int)lead.Role);
            }

            _connection.Execute(
                _updateListRoleLeadsProcedure,
                    new { tblLeadDto = dt.AsTableValuedParameter(_LeadDtoType) },
                    commandType: CommandType.StoredProcedure
                    );
        }

        public async Task<int> DeleteLeadAsync(int id)
        {
            return await _connection
                .ExecuteAsync(
                    _deleteLeadByIdProcedure,
                    new { id },
                    commandType: CommandType.StoredProcedure);
        }

        public async Task<LeadDto> GetLeadByIdAsync(int id)
        {
            LeadDto result = default;
            return (await _connection
                .QueryAsync<LeadDto, AccountDto, CityDto, Role, LeadDto>(
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
                    commandType: CommandType.StoredProcedure))
                .FirstOrDefault();
        }

        public async Task<LeadDto> GetLeadByEmailAsync(string email)
        {
            return (await _connection
                .QueryAsync<LeadDto, Role, LeadDto>(
                    _getLeadByEmailProcedure,
                    (lead, role) =>
                    {
                        lead.Role = role;
                        return lead;
                    },
                    new { email },
                    commandType: CommandType.StoredProcedure))
                .FirstOrDefault();
        }

        public async Task<List<LeadDto>> GetAllLeadsAsync()
        {
            var leadDictionary = new Dictionary<int, LeadDto>();

            return (await _connection
                .QueryAsync<LeadDto, AccountDto, CityDto, Role, LeadDto>(
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
                    commandType: CommandType.StoredProcedure))
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

        public List<LeadDto> GetAllLeadsByBatches(int lastLeadId)
        {
            var leadDictionary = new Dictionary<int, LeadDto>();

            return _connection
                .Query<LeadDto, AccountDto, Role, LeadDto>(
                    _getAllLeadsByBatchesProcedure,
                    (lead, account, role) =>
                    {

                        if (!leadDictionary.TryGetValue(lead.Id, out var leadEntry))
                        {
                            leadEntry = lead;
                            leadEntry.Role = role;
                            leadEntry.Accounts = new List<AccountDto>();
                            leadDictionary.Add(lead.Id, leadEntry);
                        }
                        leadEntry.Accounts.Add(account);

                        return leadEntry;
                    },
                    new { lastLeadId },
                    splitOn: "id",
                    commandType: CommandType.StoredProcedure)
                .Distinct()
                .ToList();
        }
    }
}