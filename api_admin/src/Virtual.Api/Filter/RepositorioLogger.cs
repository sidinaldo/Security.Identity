using Serilog;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Virtual.Api.Filter
{
    public class RepositorioLogger
    {
        private string ConnectionString { get; set; }


        public RepositorioLogger(string connection)
        {
            ConnectionString = connection;

        }

        private bool ExecuteNonQuery(string commandStr, List<SqlParameter> paramList)
        {
            var result = false;
            using (var conn = new SqlConnection(ConnectionString))
            {
                if (conn.State != System.Data.ConnectionState.Open)
                {
                    conn.Open();
                }

                using (var command = new SqlCommand(commandStr, conn))
                {
                    command.Parameters.AddRange(paramList.ToArray());
                    var count = command.ExecuteNonQuery();
                    result = count > 0;
                }
            }
            return result;
        }

        public void InsertLog(ApplicationLog log)
        {

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("Logs/log.txt",
                rollOnFileSizeLimit: true)
            .CreateLogger();

            Log.Information(log.Message);

            Log.CloseAndFlush();
        }
    }
}
