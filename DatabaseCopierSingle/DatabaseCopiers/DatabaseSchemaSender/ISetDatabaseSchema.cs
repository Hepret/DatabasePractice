using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaSender
{
    public interface ISetDatabaseSchema
    {
        void SetSchema(ScriptForSetSchema scriptForSetSchema);
    }
}