using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers;
using DatabaseCopierSingle.ScriptCreators.DatabaseDataInsertingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertData;
using DatabaseCopierSingle.ScriptCreators.ScriptsCreatorForInsertingDatabaseData;

namespace DatabaseCopierSingle.DatabaseCopiers
{
    class CopyMssqlToMssql : DatabaseCopier
    {
        public CopyMssqlToMssql(string connectionStringFrom, string connectionStringTo, bool needToCreateDatabase) :
            base(new MssqlProvider(connectionStringFrom),
                new MssqlProvider(connectionStringTo), needToCreateDatabase)
        {
            _schemaReceiver = new MssqlSchemaReceiver(ProviderFrom);
            _dataReceiver = new MssqlDataReceiver(ProviderFrom);

            _databaseSchemaInserter = new DatabaseSchemaInserter.DatabaseSchemaInserter(ProviderTo, NeedToCreateNewDatabase);
            _databaseDataInserter = new DatabaseDataInserter.DatabaseDataInserter(ProviderTo);

            _schemaScriptsCreator = new CreatorScriptsFromSchemaMssqlToMssql();
            _dataScriptsCreator = new CreatorScriptsesForInsertDataMssqlToMssql();
        }

    }
}

