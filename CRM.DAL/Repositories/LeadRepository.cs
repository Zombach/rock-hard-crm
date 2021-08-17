using CRM.DAL.Enums;
using CRM.DAL.Models;
using Dapper;
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

        public LeadRepository() { }

        public int AddLead(LeadDto dto)
        {
            return _connection.QuerySingle<int>(
                _insertLeadProcedure,
                new
                {
                    dto.FirstName,
                    dto.LastName,
                    dto.Patronymic,
                    dto.Email,
                    dto.PhoneNumber,
                    dto.Password,
                    role = (int)dto.Role,
                    cityId = dto.City.Id
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public void UpdateLead(LeadDto dto)
        {
            _connection.Execute(
                _updateLeadProcedure,
                new
                {
                    dto.Id,
                    dto.FirstName,
                    dto.LastName,
                    dto.Patronymic,
                    dto.Email,
                    dto.PhoneNumber,
                    cityId = dto.City.Id
                },
                commandType: CommandType.StoredProcedure
            );
        }

        public List<LeadDto> GetAllLeads()
        {
            var leadDictionary = new Dictionary<int, LeadDto>();
            return _connection
                .Query<LeadDto, AccountDto, CityDto, Role, LeadDto>(
                _getAllLeadsProcedure,
                (lead, account, city, role) =>
                {
                    if (leadDictionary.TryGetValue(lead.Id, out var leadDto))
                    {
                        lead = leadDto;
                    }
                    else
                    {
                        leadDictionary.Add(lead.Id, lead);
                        lead.Accounts = new List<AccountDto>();
                    }
                    lead.Role = role;
                    lead.City = city;
                    lead.Accounts.Add(account);
                    return lead;
                },
                commandType: CommandType.StoredProcedure)
                .Distinct()
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
    }
}