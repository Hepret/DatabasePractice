using DatabaseCopierSingle.DatabaseProviders;
using System;
using DatabaseCopierSingle.DatabaseCopiers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataInserter;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaInserter;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers;
using DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.ScriptsCreatorForInsertingDatabaseData;

namespace DatabaseCopierSingle
{
    class Program
    {
        
        static void Main(string[] args)
        {
            //var databaseName = "holding_auctions";
            var databaseName = "holding_auctions";
            var connectionString = $"Host = localhost; Username = postgres; Password = xlife33xlife33; Database = {databaseName}";
            var connectionStringTo = $"Host = localhost; Username = postgres; Password = xlife33xlife33;";
            const string connection_str = "Server=(localdb)\\mssqllocaldb;Integrated Security=SSPI; pooling=false; database=HoldingAuctions";
            const string connection_str2 = "Server=(localdb)\\mssqllocaldb;Integrated Security=SSPI; pooling=false";
            const  string testConnectionStringToSelfReference = "Host = localhost; Username = postgres; Password = xlife33xlife33; Database = self_reference_test";
            try
            {
                var copier = new CopyMssqlToMssql(connection_str, connection_str2, true)
                {
                    DatabaseNewName = "dwd"
                };
                copier.Copy();


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
        }

    }
}
