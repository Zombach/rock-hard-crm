using System.Data;
using System.Data.SqlClient;

namespace CRM.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected const string _connectionString =
            //@"Data Source=(localdb)\ProjectsV13;Initial Catalog=CRM.DB;";
            //@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=LeadAccount; Persist Security Info=False;";
            @"Data Source=80.78.240.16;Initial Catalog = CRM.Db; Persist Security Info=True;User ID = student;Password=qwe!23;";
        protected IDbConnection _connection;
        protected BaseRepository()
        {
            _connection = new SqlConnection(_connectionString);
        }
    }
}