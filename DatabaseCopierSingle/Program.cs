/*using DatabaseCopierSingle.DatabaseCopiers;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle
{
    public static class DefaultStringTranslator
    {
        *//*public string Translate(string defaultValue)
        {
            Stack<string> stack = new Stack<string>();
            List<string> operationList = new List<string>();
            for

        }
        private string MakeCast(string expression, string type)
        {
            return $"CAST ({expression} AS {type})";
        }*//*
    }
    class Program
    {
        
        static void Main(string[] args)
        {
            var databaseName = "holding_auctions";
            var connectionString = $"Host = localhost; Username = postgres; Password = xlife33xlife33; Database = {databaseName}";
            var connectionStringTo = $"Host = localhost; Username = postgres; Password = xlife33xlife33;";
            const string connection_str = "Server=(localdb)\\mssqllocaldb;Integrated Security=SSPI; pooling=false; database=Lol";
            const string connection_str2 = "Server=(localdb)\\mssqllocaldb;Integrated Security=SSPI; pooling=false";


            var copier = new CopyMSSQLToMSSQL(connection_str, connection_str2)
            {
                CreateNewDatabase = true,
                NewDatabaseName = "dwd"
            };

            var copier2 = new CopyPostresqlToPostgresql(connectionString, connectionStringTo)
            {
                CreateNewDatabase = true,
                NewDatabaseName = "dwd"
            };

            try
            {
                *//*var provider = new PostgresqlProvider(connectionString);
                var schema = provider.GetDatabaseSchema();
                var script = CreatorScriptsFromSchemaPostgresqlToPostgresql.CreateScript(schema);*//*

                copier2.CopySchema();
                copier2.CopyData();
            }
            catch (Exception e)
            {
                while (e != null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Console.WriteLine(e.StackTrace);
                    Console.WriteLine("##################################################");
                    e = e.InnerException;
                }
            }

            Console.WriteLine("Success");
            Console.Read();

        }

    }
}
*/