using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertData;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseDataInserter
{
    public class DatabaseDataInserter : IInsertDatabaseData
    {
        private readonly DatabaseProvider _provider;

        public DatabaseDataInserter(DatabaseProvider provider)
        {
            _provider = provider;
        }

        public void InsertData(DataInsertScripts scripts)
        {
            foreach (TableDataInsertScript tableScripts in scripts)
            {
                tableScripts.Scripts.ForEach(sc => _provider.ExecuteCommand(sc));
            }
        }
    }
}