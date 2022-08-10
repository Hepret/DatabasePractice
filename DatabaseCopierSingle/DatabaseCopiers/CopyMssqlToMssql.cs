using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers;
using DatabaseCopierSingle.ScriptCreators.DatabaseDataInsertingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator;

namespace DatabaseCopierSingle.DatabaseCopiers
{
    class CopyMssqlToMssql : DatabaseCopier
    {
        public CopyMssqlToMssql(string connectionStringFrom, string connectionStringTo, bool needToCreateDatabase) :
            base(new MssqlProvider(connectionStringFrom),
                new MssqlProvider(connectionStringTo), needToCreateDatabase)
        {
            SchemaReceiver = new MssqlSchemaReceiver(ProviderFrom);
            DataReceiver = new MssqlDataReceiver(ProviderFrom);

            DatabaseSchemaInserter = new DatabaseSchemaInserter.DatabaseSchemaInserter(ProviderTo, NeedToCreateNewDatabase);
            DatabaseDataInserter = new DatabaseDataInserter.DatabaseDataInserter(ProviderTo);

            SchemaScriptsCreator = new CreatorScriptsFromSchemaMssqlToMssql();
            DataScriptsCreator = new CreatorScriptsForInsertDataMssqlToMssql();
        }

    }
}

