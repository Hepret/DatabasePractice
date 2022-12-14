using System;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaInserter
{
    public class DatabaseSchemaInserter : IInsertDatabaseSchema
    {
        private readonly DatabaseProvider _provider;
        private bool NeedToCreateDatabase { get; set; } = false;

        private DatabaseSchemaInserter(DatabaseProvider provider)
        {
            _provider = provider;
        }
        public DatabaseSchemaInserter(DatabaseProvider provider, bool needToCreateDatabase = false) : this(provider)
        {
            NeedToCreateDatabase = needToCreateDatabase;
        }
        public void SetSchema(DatabaseSchemaCreatingScript databaseSchemaCreatingScript)
        {
            CreateDatabase(databaseSchemaCreatingScript.CreateDatabaseScript);
            CreateExtensions(databaseSchemaCreatingScript.CreateExtensionScript);
            CreateSchemas(databaseSchemaCreatingScript.CreateSchemasScripts);
            CreateSequences(databaseSchemaCreatingScript.CreateSequencesScripts);
            CreateTables(databaseSchemaCreatingScript.CreateTablesScripts);
        }

        private void CreateExtensions(string[] createExtensionScripts)
        {
            if (createExtensionScripts == null) return;
            foreach (var createExtensionScript in createExtensionScripts)
            {
                _provider.ExecuteCommand(createExtensionScript);
            }
        }

        private void CreateTables(CreateTablesScripts createTablesScripts)
        {
            foreach (CreateTableScript createTableScripts in createTablesScripts)
            {
                _provider.ExecuteCommand(createTableScripts.Script);
            }
        }
        private void CreateSequences(string[] createSequencesScripts)
        {
            foreach (var createSequencesScript in createSequencesScripts)
            {
                _provider.ExecuteCommand(createSequencesScript);
            }
        }
        private void CreateDatabase(CreateDatabaseScript createDatabaseScript)
        {
            if (createDatabaseScript == null) return;
            _provider.ExecuteCommand(createDatabaseScript.Script);
            var databaseNameNew = createDatabaseScript.DatabaseName;
            _provider.ChangeDatabase(databaseNameNew);
        }
        private void CreateSchemas(string[] createSchemasScript)
        {
            foreach (var createSchemaScript in createSchemasScript)
            {
                _provider.ExecuteCommand(createSchemaScript);
            }
        }
        
    }
}