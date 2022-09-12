using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators.DatabaseDataInsertingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator;

namespace DatabaseCopierSingle.DatabaseCopiers
{
    public class CopyMssqlToPostgresql : DatabaseCopier
    {
        public CopyMssqlToPostgresql(string connectionStringFrom, string connectionStringTo, bool needToCreateDatabase) :
            base(new MssqlProvider(connectionStringFrom),
                new PostgresqlProvider(connectionStringTo), needToCreateDatabase)
        {
            SchemaReceiver = new MssqlSchemaReceiver(ProviderFrom);
            DataReceiver = new MssqlDataReceiver(ProviderFrom);

            DatabaseSchemaInserter = new DatabaseSchemaInserter.DatabaseSchemaInserter(ProviderTo, NeedToCreateNewDatabase);
            DatabaseDataInserter = new DatabaseDataInserter.DatabaseDataInserter(ProviderTo);

            SchemaScriptsCreator = new CreatorScriptsFromSchemaMssqlToPostgresql();
            DataScriptsCreator = new CreatorScriptsForInsertDataMssqlToPostgresql();
        }
    }
}