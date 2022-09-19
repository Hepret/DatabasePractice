using Npgsql;
using System;
using DatabaseCopierSingle.DatabaseProviders.Exceptions;

namespace DatabaseCopierSingle.DatabaseProviders
{
    public class PostgresqlProvider : DatabaseProvider
    {
        public PostgresqlProvider(string connectionString)
        {
            ValidateConnectionString(connectionString);
            var connection = new NpgsqlConnection(connectionString);
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
                var connectionStringBuilder = new NpgsqlConnectionStringBuilder()
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
                var res = (int)(long)cmd.ExecuteScalar();
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