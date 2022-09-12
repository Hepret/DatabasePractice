using System;
using System.Data;
using System.Data.Common;

namespace DatabaseCopierSingle.DatabaseProviders
{
    public abstract class DatabaseProvider
    {
        protected readonly DbConnection Conn;

        public string DatabaseName => Conn.Database;

        protected DatabaseProvider(DbConnection connection)
        {
            Conn = connection;
            try
            {
                Conn.Open();
            }
            catch (Exception e)
            {
                Conn.Close();
                throw new Exception($"Can't connect to database, with connection string {Conn.ConnectionString}", e);
            }
        }
        public void ChangeDatabase(string databaseName)
        {
            try
            {
                if (Conn.State == ConnectionState.Closed) Conn.Open();
                Conn.ChangeDatabase(databaseName);
            }
            catch (DbException e)
            {
                Conn.Close();
                throw new Exception($"Can't change database to: {databaseName}", e);
            }
        }
        public void ExecuteCommand(string command)
        {
            try
            {
                var cmd = Conn.CreateCommand();
                cmd.CommandText = command;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Conn.Close();
                throw new Exception($"Invalid Operation:\n {command}", e);
            }

        }
        public abstract int ExecuteCommandScalar(string command);
        public DbDataReader GetDataReader(string queryString)
        {
            try
            {
                var cmd = Conn.CreateCommand();
                cmd.CommandText = queryString;
                var reader = cmd.ExecuteReader();
                return reader;
            }
            catch (Exception e)
            {
                Conn.Close();
                throw new Exception($"Invalid Operation:\n {queryString}", e);
            }
        }
    }
}
