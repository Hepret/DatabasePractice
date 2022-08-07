using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema
{
    public class CreateDatabaseSchemaScript
    {
        private readonly SchemaDatabase _schemaDatabase;

        public CreateDatabaseSchemaScript(SchemaDatabase schemaDatabase)
        {
            this._schemaDatabase = schemaDatabase;
            CreateDatabaseScript = new CreateDatabaseScript(schemaDatabase.DatabaseName);
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