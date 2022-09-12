using System;
using System.Data;
using System.Data.Common;
using DatabaseCopierSingle.DatabaseCopiers;
using DatabaseCopierSingle.DatabaseProviders.Exceptions;

namespace DatabaseCopierSingle.DatabaseProviders
{
    public abstract class DatabaseProvider
    {
        protected  DbConnection Conn;

        public string DatabaseName => Conn.Database;

        protected abstract void ValidateConnectionString(string connectionString);
        
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
