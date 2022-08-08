using DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers;
using DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;
using DatabaseCopierSingle.ScriptCreators.ScriptsCreatorForInsertingDatabaseData;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle.DatabaseCopiers
{
    abstract class DatabaseCopier : IDatabaseCopy
    {
        protected DatabaseProvider ProviderFrom;
        protected DatabaseProvider ProviderTo;

        protected DatabaseSchemaReceiver _schemaReceiver;
        protected DatabaseDataReceiver _dataReceiver;

        protected DatabaseSchemaInserter.DatabaseSchemaInserter _databaseSchemaInserter;
        protected DatabaseDataInserter.DatabaseDataInserter _databaseDataInserter;

        protected ICreateInsertSchemaScripts _schemaScriptsCreator;
        protected ICreateInsertDataScripts _dataScriptsCreator;

        protected SchemaDatabase Schema;
        protected DatabaseData Data;
        private string _databaseNewName;

        public bool NeedToCreateNewDatabase { get; } = false;

        protected DatabaseCopier(DatabaseProvider providerFrom, DatabaseProvider providerTo)
        {
            ProviderFrom = providerFrom;
            ProviderTo = providerTo;
        }
        
        protected DatabaseCopier(DatabaseProvider providerFrom, DatabaseProvider providerTo, bool needToCreateNewDatabase) : this(providerFrom, providerTo)
        {
            NeedToCreateNewDatabase = needToCreateNewDatabase;
            DatabaseNewName = providerFrom.DatabaseName.ToLower() + "_copy";
        }
        public string DatabaseNewName
        {
            get => _databaseNewName;
            set => _databaseNewName = value.ToLower();
        }
        private void CopySchema()
        {
            Schema = _schemaReceiver.GetDatabaseSchema();
            var scripts = NeedToCreateNewDatabase ? _schemaScriptsCreator.CreateScriptsForInsertSchema(Schema, DatabaseNewName) : _schemaScriptsCreator.CreateScriptsForInsertSchema(Schema);
            _databaseSchemaInserter.SetSchema(scripts);
        }
        private void CopyData()
        {
            Data = _dataReceiver.GetDatabaseData(Schema);
            var scripts = _dataScriptsCreator.CreateInsertDataScript(Data);
            _databaseDataInserter.InsertData(scripts);
        }
        public void Copy()
        {
            CopySchema();
            CopyData();
        }
    }
}
