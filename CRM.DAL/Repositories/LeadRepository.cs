using CRM.DAL.Models;
using Dapper;
using System.Data;

namespace CRM.DAL.Repositories
{
    public class LeadRepository : BaseRepository
    {
        private const string _addLeadProcedure = "";
        private const string _updateLeadProcedure = "";
        public LeadRepository() { }

        public int AddLead (LeadDto dto)
        {
            return _connection.QuerySingle<int>(
                _addLeadProcedure,
                new
                {
                    dto.FirstName ,
                    dto.LastName,
                    dto.Patronymic,
                    dto.RegistrationDate,
                    dto.Email,
                    dto.PhoneNumber,
                    dto.Password,
                    dto.Role
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
                    dto.FirstName,
                    dto.LastName,
                    dto.Patronymic,
                    dto.RegistrationDate,
                    dto.Email,
                    dto.PhoneNumber,
                    dto.Password,
                    dto.Role
                },
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
