using DatabaseCopierSingle.ScriptCreators.ScriptForInsertData;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseDataInserter
{
    public interface IInsertDatabaseData
    {
        void InsertData(DataInsertScripts scripts);
    }
}