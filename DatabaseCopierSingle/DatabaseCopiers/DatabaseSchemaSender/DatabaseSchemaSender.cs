using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaSender
{
    public class DatabaseSchemaSender : ISetDatabaseSchema
    {
        private readonly DatabaseProvider _provider;
        public bool NeedToCreateDatabase { get; set; } = false;

        public DatabaseSchemaSender(DatabaseProvider provider)
        {
            _provider = provider;
        }

        public void SetSchema(ScriptForSetSchema scriptForSetSchema)
        {
            CreateDatabase(scriptForSetSchema.CreateDatabaseScript);
            CreateSchemas(scriptForSetSchema.CreateSchemasScripts);
            CreateSequences(scriptForSetSchema.CreateSequencesScripts);
            CreateTables(scriptForSetSchema.CreateTablesScripts);
        }

        private void CreateTables(CreateTablesScripts createTablesScripts)
        {
            foreach (var createTablesScript in createTablesScripts)
            {
                _provider.ExecuteCommand((string)createTablesScript);
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