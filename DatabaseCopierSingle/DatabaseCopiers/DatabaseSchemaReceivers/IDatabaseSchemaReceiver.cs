using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers
{
    public interface IDatabaseSchemaReceiver
    {
        SchemaDatabase GetDatabaseSchema();
    }
}