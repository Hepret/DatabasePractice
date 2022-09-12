using System;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseProviders;

namespace DatabaseCopierSingle.DatabaseCopiers
{

    public static class DatabaseCopierFactory
    {
        public static DatabaseCopier CreateCopier(string connectionStringFrom, Database databaseFrom, string connectingStringTo, Database databaseTo, bool needToCreateNewDatabase)
        {
            DatabaseCopier copier;
            switch (databaseFrom)
            {
                case Database.Mssql when databaseTo == Database.Mssql:
                    copier = new CopyMssqlToMssql(connectionStringFrom, connectingStringTo, needToCreateNewDatabase);
                    break;
                case Database.Mssql when  databaseTo == Database.Postgresql:
                    copier = new CopyMssqlToPostgresql(connectionStringFrom, connectingStringTo, needToCreateNewDatabase);
                    break;
                case Database.Postgresql when databaseTo == Database.Mssql:
                    copier = new CopyPostgresqlToMssql(connectionStringFrom, connectingStringTo, needToCreateNewDatabase);
                    break;
                case Database.Postgresql when databaseTo == Database.Postgresql:
                    copier = new CopyPostgresqlToPostgresql(connectionStringFrom, connectingStringTo,
                        needToCreateNewDatabase);
                    break;
                default:
                    throw  new Exception();
            }

            return copier;
        }
    }
    public enum Database
    {
        Mssql,
        Postgresql
    }
    
}