using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator
{
    public interface ICreateInsertSchemaScripts
    {
        DatabaseSchemaCreatingScript CreateScriptsForInsertSchema(SchemaDatabase schemaDatabase, string databaseNewName);
        DatabaseSchemaCreatingScript CreateScriptsForInsertSchema(SchemaDatabase schemaDatabase);
    }
}