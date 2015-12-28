using Payroll.Repository.Entities;
using Payroll.Repository.Interface;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Payroll.Repository
{
    public class DatabaseProvider
    {
        private DbConnection _connection = null;
        private readonly IDatabase _database;

        public DatabaseProvider()
        {
            var databaseType = ConfigurationManager.AppSettings["DatabaseType"];
            var connectionString = ConfigurationManager.ConnectionStrings["Payroll.ConnectionString"].ToString();

            switch (databaseType)
            {
                case "MySql"://DatabaseEnums.MySql:
                    _database = new Entities.MySql(connectionString);
                    break;
                case "MsSql"://DatabaseEnums.MySql:
                    _database = new MsSql(connectionString);
                    break;
            }
        }

        public virtual DataSet ExecuteDataSet(string query)
        {
            return _database.ExecuteDataSet(query);
        }
        
    }

    

}
