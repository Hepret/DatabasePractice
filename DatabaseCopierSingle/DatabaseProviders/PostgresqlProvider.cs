using Npgsql;
using System;

namespace DatabaseCopierSingle.DatabaseProviders
{
    public class PostgresqlProvider : DatabaseProvider
    {
        public PostgresqlProvider(string connectionString) : base(new NpgsqlConnection(connectionString)) { }
        

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