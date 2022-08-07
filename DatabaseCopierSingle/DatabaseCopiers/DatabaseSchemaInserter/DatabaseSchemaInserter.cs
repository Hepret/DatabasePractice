﻿using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaInserter
{
    public class DatabaseSchemaInserter : IInsertDatabaseSchema
    {
        private readonly DatabaseProvider _provider;
        private bool NeedToCreateDatabase { get; set; } = false;
        public DatabaseSchemaInserter(DatabaseProvider provider)
        {
            _provider = provider;
        }
        public DatabaseSchemaInserter(DatabaseProvider provider, bool needToCreateDatabase) : this(provider)
        {
            NeedToCreateDatabase = needToCreateDatabase;
        }
        public void SetSchema(CreateDatabaseSchemaScript createDatabaseSchemaScript)
        {
            CreateDatabase(createDatabaseSchemaScript.CreateDatabaseScript);
            CreateSchemas(createDatabaseSchemaScript.CreateSchemasScripts);
            CreateSequences(createDatabaseSchemaScript.CreateSequencesScripts);
            CreateTables(createDatabaseSchemaScript.CreateTablesScripts);
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
            if (NeedToCreateDatabase) _provider.ExecuteCommand(createDatabaseScript.Script);
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