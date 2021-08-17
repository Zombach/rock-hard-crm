using System.Data;
using System.Data.SqlClient;

namespace CRM.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected const string _connectionString = "Data Source=DESKTOP-2FBGSPQ;Initial Catalog=CRM;Integrated Security=True;";
        protected IDbConnection _connection;
        protected BaseRepository()
        {
            _connection = new SqlConnection(_connectionString);
        }
    }
}
