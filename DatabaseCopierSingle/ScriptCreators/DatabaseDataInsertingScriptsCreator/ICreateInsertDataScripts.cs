using DatabaseCopierSingle.ScriptCreators.ScriptForInsertData;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptsCreatorForInsertingDatabaseData
{
    public interface ICreateInsertDataScripts
    {
        DataInsertScripts CreateInsertDataScript(DatabaseData data);
    }
}