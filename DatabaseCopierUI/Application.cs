using System;
using System.Data.Common;
using DatabaseCopierSingle.DatabaseCopiers;

namespace DatabaseCopierUI
{
    internal static class Application
    {
        private static readonly int DatabaseCount = Enum.GetNames(typeof(Database)).Length;
        private static Database DatabaseFrom { get; set; }
        private static string ConnectionStringFrom { get; set; }
        private static Database DatabaseTo { get; set; }
        private static string ConnectionStringTo { get; set;}
        private static bool NeedToCreateDatabase { get; set;}
        private static string DatabaseNewName { get; set; }

        public static void Run()
        {
            var copier = CreateCopier();
            copier.Copy();
            Console.WriteLine("Success");
        }

        private static DatabaseCopier CreateCopier()
        {
            GetDataFromUser();

            try
            {
                var copier =  DatabaseCopierFactory
                    .CreateCopier(connectionStringFrom: ConnectionStringFrom,
                        databaseFrom: DatabaseFrom,
                        connectingStringTo: ConnectionStringTo,
                        databaseTo: DatabaseTo,
                        needToCreateNewDatabase: NeedToCreateDatabase);
                if (!string.IsNullOrEmpty(DatabaseNewName))
                {
                    copier.DatabaseNewName = DatabaseNewName;
                }
            
                return copier;
            }
            catch (Exception)
            {
                Console.WriteLine("Data entry error, please enter data again");
                return CreateCopier();
            }
        }

        private static void GetDataFromUser()
        {
            GetDatabaseFrom();
            GetConnectionStringFrom();
            GetDatabaseTo();
            GetConnectionStringTo();
            NeedToCreateNewDatabase();
            GetDatabaseNewName();
        }
        
        private static void GetConnectionStringTo()
        {
            Console.WriteLine("Enter connection string to the DBMS to which the data will be copied ");
            ConnectionStringTo = GetConnectionString();
        }

        private static void GetDatabaseTo()
        {   
            Console.WriteLine("Enter the number of the DBMS to which the data will be copied:");
            DatabaseTo = GetDatabase();
        }

        private static void GetConnectionStringFrom()
        {
            Console.WriteLine("Enter connection string to the DBMS from which the data will be copied: ");
            ConnectionStringFrom = GetConnectionString();
        }

        private static void GetDatabaseFrom()
        {
            Console.WriteLine("Enter the number of the DBMS from which the data will be copied:");
            DatabaseFrom = GetDatabase();
        }
        private static void NeedToCreateNewDatabase()
        {
            Console.WriteLine("Enter whether to create a new database (Y/N)");
            var userInput = Console.ReadLine();
            while (string.IsNullOrEmpty(userInput) || (userInput.ToUpper() == "Y" || userInput.ToUpper() == "N"))
            {
                Console.WriteLine("Enter Y/N");
            }

            NeedToCreateDatabase = 
                userInput.ToUpper() == "Y";
        }

        private static void GetDatabaseNewName()
        {
            if (!NeedToCreateDatabase) return;
            Console.Write("Enter new database name:");
            DatabaseNewName = Console.ReadLine();
        }
        
        private static string GetConnectionString()
        {
            Console.Write("Connection string: ");
            var userInput = Console.ReadLine();

            try
            {
                var _ = new DbConnectionStringBuilder
                {
                    ConnectionString = userInput ?? throw new InvalidOperationException()
                };
            }
            catch (Exception)
            {
                Console.WriteLine("Enter correct connection string");
                return GetConnectionString();
            }

            return userInput;
        }

        private static Database GetDatabase() => (Database) GetDatabaseNumber();

        private static int GetDatabaseNumber()
        {
            int databaseNumber = -1;
            for (int i = 0; i < DatabaseCount ; i++)
            {
                Console.WriteLine($"{i}) {(Database)i}");
            }

            var successToReadNumber = false;
            while (successToReadNumber is false)
            {
                successToReadNumber = int.TryParse(Console.ReadLine(), out  databaseNumber);
                if (successToReadNumber)
                {
                    var databaseNumberIsExist = databaseNumber.Between(0, DatabaseCount - 1);
                    if (databaseNumberIsExist is false) successToReadNumber = false;
                }
                
                if (successToReadNumber is false)
                {
                    Console.WriteLine("Enter correct number");
                }
            }

            return databaseNumber;
        }
    }
}