using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertData
{
    public class DataInsertScripts : IEnumerable
    {
        public TableDataInsertScript[] Scripts { get; private set; }
        private SchemaDatabase _schemaDatabase { get; set; }

        public DataInsertScripts(SchemaDatabase schemaDatabase)
        {
            _schemaDatabase = schemaDatabase;
            Scripts = new TableDataInsertScript[_schemaDatabase.Tables.Count];
            for (int i = 0; i < _schemaDatabase.Tables.Count; i++)
            {
                Scripts[i] = new TableDataInsertScript(_schemaDatabase.Tables[i]);
            }
        }

        public TableDataInsertScript this[int index] => Scripts[index];

        public TableDataInsertScript this[FullTableName fullTableName] => Scripts.First(sc => Equals(sc.FullTableName, fullTableName));
        
        #region Add scripts that insert data in tables

        public void AddTableDataInsertScripts(int index,IEnumerable<string> scripts)
        {
            this[index].AddScripts(scripts);
        }
        public void AddTableDataInsertScripts(FullTableName fullTableName,IEnumerable<string> scripts)
        {
            this[fullTableName].AddScripts(scripts);
        }

        public void AddTableDataInsertScript(int index, string script)
        {
            this[index].AddScript(script);
        }

        public void AddTableDataInsertScript(FullTableName fullTableName, string script)
        {
            this[fullTableName].AddScript(script);
        }        

        #endregion
        

        public IEnumerator GetEnumerator()
        {
            return Scripts.GetEnumerator();
        }
    }
}