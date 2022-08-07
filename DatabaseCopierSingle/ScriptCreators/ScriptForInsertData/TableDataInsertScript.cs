using System.Collections;
using System.Collections.Generic;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertData
{
    public class TableDataInsertScript : IEnumerable
    {
        private readonly SchemaTable _schemaTable;
        public FullTableName FullTableName => _schemaTable.FullTableName;
        public List<string> Scripts { get; private set; }
        
        public TableDataInsertScript(SchemaTable schemaTable)
        {
            _schemaTable = schemaTable;
            Scripts = new List<string>();
        }

        public string this[int index] => Scripts[index];


        public void AddScript(string script)
        {
            Scripts.Add(script);
        }

        public void AddScripts(IEnumerable<string> scripts)
        {
            Scripts.AddRange(scripts);
        }

        public IEnumerator GetEnumerator()
        {
            return Scripts.GetEnumerator();
        }
    }
}