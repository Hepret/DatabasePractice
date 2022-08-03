using DatabaseCopierSingle.DatabaseCopiers;
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
    class Program
    {
        
        static void Main(string[] args)
        {
            //var databaseName = "holding_auctions";
            var databaseName = "dwd";
            var connectionString = $"Host = localhost; Username = postgres; Password = xlife33xlife33; Database = {databaseName}";
            var connectionStringTo = $"Host = localhost; Username = postgres; Password = xlife33xlife33;";
            const string connection_str = "Server=(localdb)\\mssqllocaldb;Integrated Security=SSPI; pooling=false; database=Lol";
            const string connection_str2 = "Server=(localdb)\\mssqllocaldb;Integrated Security=SSPI; pooling=false";
            const  string testConnectionStringToSelfReference = "Host = localhost; Username = postgres; Password = xlife33xlife33; Database = self_reference_test";
            try
            {
                var copier = new CopyPostresqlToPostgresql(connectionString, connectionStringTo)
                {
                    CreateNewDatabase = true,
                    NewDatabaseName = "DeleteMe"
                };
                copier.CopySchema();
                copier.CopyData();


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
