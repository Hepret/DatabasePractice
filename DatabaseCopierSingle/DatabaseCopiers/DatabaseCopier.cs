using System.Linq;
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
        protected readonly DatabaseProvider ProviderFrom;
        protected readonly DatabaseProvider ProviderTo;

        protected DatabaseSchemaReceiver SchemaReceiver;
        protected DatabaseDataReceiver DataReceiver;

        protected DatabaseSchemaInserter.DatabaseSchemaInserter DatabaseSchemaInserter;
        protected DatabaseDataInserter.DatabaseDataInserter DatabaseDataInserter;

        protected ICreateInsertSchemaScripts SchemaScriptsCreator;
        protected ICreateInsertDataScripts DataScriptsCreator;

        protected SchemaDatabase Schema;
        protected DatabaseData Data;
        private string _databaseNewName;

        protected bool NeedToCreateNewDatabase { get; } = false;

        private DatabaseCopier(DatabaseProvider providerFrom, DatabaseProvider providerTo)
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
            Schema = SchemaReceiver.GetDatabaseSchema();
            var scripts = NeedToCreateNewDatabase ? SchemaScriptsCreator.CreateScriptsForInsertSchema(Schema, DatabaseNewName) : SchemaScriptsCreator.CreateScriptsForInsertSchema(Schema);
            DatabaseSchemaInserter.SetSchema(scripts);
        }
        private void CopyData()
        {
            Data = DataReceiver.GetDatabaseData(Schema);
            var scripts = DataScriptsCreator.CreateInsertDataScript(Data);
            DatabaseDataInserter.InsertData(scripts);
        }
        

        public void Copy()
        {
            CopySchema();
            CopyData();
        }
    }
}
