using MySql.Data.MySqlClient;
using Payroll.Repository.Interface;
using System.Data;
using System.Data.Common;

namespace Payroll.Repository.Entities
{
    public class MySql : IDatabase
    {
        private readonly string _connectionString;
        private MySqlConnection _connection = null;


        public MySql(string connectionString)
        {
            _connectionString = connectionString;
        }

        public virtual DbConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        } 
        

        public virtual DataSet ExecuteDataSet(string query)
        {

            using(_connection = GetConnection() as MySqlConnection)
            {
                _connection.Open();
                using(var cmd = new MySqlCommand(query, _connection))
                {
                    using(var adapter = new MySqlDataAdapter(cmd))
                    {
                        using(var ds = new DataSet())
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
