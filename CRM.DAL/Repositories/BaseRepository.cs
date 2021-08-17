
using System.Data;
using System.Data.SqlClient;

namespace CRM.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected const string _connectionString = "";
        protected IDbConnection _connection;
        protected BaseRepository()
        {
            _connection = new SqlConnection(_connectionString);
        }
    }
}
