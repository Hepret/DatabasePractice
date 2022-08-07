using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaInserter
{
    public interface IInsertDatabaseSchema
    {
        void SetSchema(CreateDatabaseSchemaScript createDatabaseSchemaScript);
    }
}