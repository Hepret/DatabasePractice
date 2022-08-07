using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema
{
    public class CreateTablesScripts : IEnumerable<CreateTableScript>
    {
        private readonly List<SchemaTable> _tables;
        private CreateTableScript[] Scripts { get;}

        public int Count => Scripts.Length;

        public CreateTablesScripts(List<SchemaTable> tables)
        {
            _tables = tables;
            Scripts = new CreateTableScript[tables.Count];
            for (int i = 0; i < Scripts.Length; i++)
            {
                Scripts[i] = new CreateTableScript(_tables[i]);
            }
            
        }
        public CreateTableScript this[int index] => Scripts[index];
        public CreateTableScript this[FullTableName fullTableName] => Scripts.First(t => t.FullTableName.Equals(fullTableName));


        public IEnumerator<CreateTableScript> GetEnumerator()
        {
            return ((IEnumerable<CreateTableScript>) Scripts).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}