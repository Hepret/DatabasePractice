using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema
{
    public class DatabaseSchemaCreatingScript
    {
        private readonly SchemaDatabase _schemaDatabase;

        public DatabaseSchemaCreatingScript(SchemaDatabase schemaDatabase, string databaseNewName)
        {
            this._schemaDatabase = schemaDatabase;
            CreateDatabaseScript = new CreateDatabaseScript(databaseNewName);
            CreateSchemasScripts = new string[schemaDatabase.Schemas.Count];
            CreateSequencesScripts = new string[schemaDatabase.Sequences.Count];
            CreateTablesScripts = new CreateTablesScripts(_schemaDatabase.Tables);
        }

        public DatabaseSchemaCreatingScript(SchemaDatabase schemaDatabase)
        {
            this._schemaDatabase = schemaDatabase;
            CreateSchemasScripts = new string[schemaDatabase.Schemas.Count];
            CreateSequencesScripts = new string[schemaDatabase.Sequences.Count];
            CreateTablesScripts = new CreateTablesScripts(_schemaDatabase.Tables);
        }

        public CreateDatabaseScript CreateDatabaseScript { get; set; }
        public string[]  CreateSchemasScripts{ get; set; }
        public string[] CreateSequencesScripts { get; set; }
        public CreateTablesScripts CreateTablesScripts { get; set; }

    }

    
}