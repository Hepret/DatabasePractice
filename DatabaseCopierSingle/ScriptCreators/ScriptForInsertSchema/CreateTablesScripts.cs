using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema
{
    public class CreateTablesScripts : IEnumerable
    {
        private readonly List<SchemaTable> _tables;
        private string[] Scripts { get; }
        public CreateTablesScripts(List<SchemaTable> tables)
        {
            _tables = tables;
            Scripts = new string[tables.Count];
        }
        public string this[int index]
        {
            set => Scripts[index] = value;
            get => Scripts[index];
        }
        public string this[string tableName]
        {
            get
            {
                var searchingTable = _tables.First(table => table.TableName == tableName);
                var index = _tables.IndexOf(searchingTable);
                return Scripts[index];
            }
            set
            {
                var searchingTable = _tables.First(table => table.TableName == tableName);
                var index = _tables.IndexOf(searchingTable);
                Scripts[index] = value;
            }
            
        }
        
        public IEnumerator GetEnumerator()
        {
            return  Scripts.GetEnumerator();
        }
    }
}