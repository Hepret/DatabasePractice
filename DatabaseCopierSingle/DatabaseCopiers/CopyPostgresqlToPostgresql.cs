using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators.DatabaseDataInsertingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.ScriptsCreatorForInsertingDatabaseData;

namespace DatabaseCopierSingle.DatabaseCopiers
{
    class CopyPostgresqlToPostgresql : DatabaseCopier
    {
        public CopyPostgresqlToPostgresql(string connectionStringFrom, string connectionStringTo, bool needToCreateDatabase) :
            base(new PostgresqlProvider(connectionStringFrom),
                new PostgresqlProvider(connectionStringTo), needToCreateDatabase)
        {
            SchemaReceiver = new PostgresqlSchemaReceiver(ProviderFrom);
            DataReceiver = new PostgresqlDataReceiver(ProviderFrom);

            DatabaseSchemaInserter = new DatabaseSchemaInserter.DatabaseSchemaInserter(ProviderTo, NeedToCreateNewDatabase);
            DatabaseDataInserter = new DatabaseDataInserter.DatabaseDataInserter(ProviderTo);

            SchemaScriptsCreator = new CreatorScriptsFromSchemaPostgresqlToPostgresql();
            DataScriptsCreator = new CreatorScriptsForInsertDataPostgresqlToPostgresql();
        }
        
    }
}
