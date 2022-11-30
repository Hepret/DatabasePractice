using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema
{
    public class DatabaseSchemaCreatingScript
    {
        private readonly SchemaDatabase _schemaDatabase;

        public DatabaseSchemaCreatingScript(SchemaDatabase schemaDatabase, string databaseNewName) : this(schemaDatabase)
        {
            CreateDatabaseScript = new CreateDatabaseScript(databaseNewName);
        }

        public DatabaseSchemaCreatingScript(SchemaDatabase schemaDatabase)
        {
            this._schemaDatabase = schemaDatabase;
            CreateExtensionsScripts = new string[schemaDatabase.Extensions?.Count ?? 0];
            CreateSchemasScripts = new string[schemaDatabase.Schemas.Count];
            CreateSequencesScripts = new string[schemaDatabase.Sequences.Count];
            CreateTablesScripts = new CreateTablesScripts(_schemaDatabase.Tables);
        }

        public CreateDatabaseScript CreateDatabaseScript { get; set; }
        public string[]  CreateSchemasScripts{ get; set; }
        public string[] CreateSequencesScripts { get; set; }
        public string[] CreateExtensionsScripts { get; set; }
        public CreateTablesScripts CreateTablesScripts { get; set; }
        public string[] CreateExtensionScript { get; set; }
    }

    
}