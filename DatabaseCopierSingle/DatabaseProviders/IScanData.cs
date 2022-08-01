using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle.DatabaseProviders
{
    internal interface IScanData
    {
        DatabaseData GetData(SchemaDatabase schema);
    }
}