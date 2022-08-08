using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers;
using DatabaseCopierSingle.DatabaseProviders;
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
            _schemaReceiver = new PostgresqlSchemaReceiver(ProviderFrom);
            _dataReceiver = new PostgresqlDataReceiver(ProviderFrom);

            _databaseSchemaInserter = new DatabaseSchemaInserter.DatabaseSchemaInserter(ProviderTo, NeedToCreateNewDatabase);
            _databaseDataInserter = new DatabaseDataInserter.DatabaseDataInserter(ProviderTo);

            _schemaScriptsCreator = new CreatorScriptsFromSchemaPostgresqlToPostgresql();
            _dataScriptsCreator = new CreatorScriptsesForInsertDataPostgresqlToPostgresql();
        }
        
    }
}
