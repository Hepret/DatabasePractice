using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers
{
    public interface IDatabaseDataReceiver
    {
         DatabaseData GetDatabaseData(SchemaDatabase schemaDatabase);
    }
}