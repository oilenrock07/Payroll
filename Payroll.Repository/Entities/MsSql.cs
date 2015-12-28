using System.Data.SqlClient;
using Payroll.Repository.Interface;
using System.Data.Common;
using System.Data;

namespace Payroll.Repository.Entities
{
    public class MsSql : IDatabase
    {
        private readonly string _connectionString;
        private SqlConnection _connection = null;

        public MsSql(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        public virtual DataSet ExecuteDataSet(string query)
        {

            using (_connection = GetConnection() as SqlConnection)
            {
                _connection.Open();
                using (var cmd = new SqlCommand(query, _connection))
                {
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        using (var ds = new DataSet())
                        {
                            adapter.Fill(ds);
                            return ds;
                        }
                    }
                }
            }
        }
    }
}
