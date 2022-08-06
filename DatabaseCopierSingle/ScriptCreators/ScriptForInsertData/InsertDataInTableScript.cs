using System.Collections;
using System.Collections.Generic;
using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertData
{
    public class InsertDataInTableScript : IEnumerable
    {
        private readonly SchemaTable _schemaTable;

        public string TableName => _schemaTable.TableName;

        private List<string> InsertDataInScripts { get; set; }
        
        public InsertDataInTableScript(SchemaTable schemaTable)
        {
            _schemaTable = schemaTable;
            InsertDataInScripts = new List<string>();
        }

        public string this[int index] => InsertDataInScripts[index];


        public void AddScript(string script)
        {
            InsertDataInScripts.Add(script);
        }

        public void AddScripts(IEnumerable<string> scripts)
        {
            InsertDataInScripts.AddRange(scripts);
        }

        public IEnumerator GetEnumerator()
        {
            return InsertDataInScripts.GetEnumerator();
        }
    }
}