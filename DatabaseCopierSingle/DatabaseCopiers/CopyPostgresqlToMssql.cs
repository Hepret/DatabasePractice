using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators.DatabaseDataInsertingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator;

namespace DatabaseCopierSingle.DatabaseCopiers
{
    public class CopyPostgresqlToMssql : DatabaseCopier
    {
        public CopyPostgresqlToMssql(string connectionStringFrom, string connectionStringTo, bool needToCreateDatabase) :
            base(new PostgresqlProvider(connectionStringFrom),
                new MssqlProvider(connectionStringTo), needToCreateDatabase)
        {
            SchemaReceiver = new PostgresqlSchemaReceiver(ProviderFrom);
            DataReceiver = new PostgresqlDataReceiver(ProviderFrom);

            DatabaseSchemaInserter = new DatabaseSchemaInserter.DatabaseSchemaInserter(ProviderTo, NeedToCreateNewDatabase);
            DatabaseDataInserter = new DatabaseDataInserter.DatabaseDataInserter(ProviderTo);

            SchemaScriptsCreator = new CreatorScriptsFromSchemaPostgresqlToMssql();
            DataScriptsCreator = new CreatorScriptsForInsertDataPostgresqlToMssql();
        }
    }
}