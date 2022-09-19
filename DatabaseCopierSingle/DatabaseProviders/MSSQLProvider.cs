using System;
using System.Data.SqlClient;
using DatabaseCopierSingle.DatabaseProviders.Exceptions;

namespace DatabaseCopierSingle.DatabaseProviders
{
    public class MssqlProvider : DatabaseProvider
    {
        public MssqlProvider(string connectionString)
        {
            ValidateConnectionString(connectionString);
            SqlConnection connection = new SqlConnection(connectionString);
            Conn = connection;
            try
            {
                Conn.Open();
            }
            catch (Exception e)
            {
                Conn.Close();
                throw new DatabaseConnectionFailedException($"Can't connect to database, with connection string {Conn.ConnectionString}", e);
            }
        }

        protected sealed override void ValidateConnectionString(string connectionString)
        {
            try
            {
                var connectionStringBuilder = new SqlConnectionStringBuilder()
                {
                    ConnectionString = connectionString
                };
            }
            catch (Exception e)
            {
                throw new ConnectionStringFormatException("Invalid connection string format",e);
            }
        }

        public override int ExecuteCommandScalar(string command)
        {
            try
            {
                var cmd = Conn.CreateCommand();
                cmd.CommandText = command;
                var res = (int)cmd.ExecuteScalar();
                return res;
            }
            catch (Exception e)
            {
                Conn.Close();
                throw new Exception($"Invalid Operation:\n {command}", e);
            }

        }
    }
}
