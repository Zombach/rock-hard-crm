using CRM.Core;
using Microsoft.Extensions.Options;
using System.Data;
using System.Data.SqlClient;

namespace CRM.DAL.Repositories
{
    public abstract class BaseRepository
    {
        protected IDbConnection _connection;
        protected BaseRepository(IOptions<DatabaseSettings> options)
        {
            _connection = new SqlConnection(options.Value.ConnectionString);
        }
    }
}