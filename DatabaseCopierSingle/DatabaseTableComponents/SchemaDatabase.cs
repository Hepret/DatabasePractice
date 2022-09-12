using System.Collections.Generic;
using System.Linq;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class SchemaDatabase
    {
        List<SchemaTable> _tables;
        public string DatabaseName { get; set; }
        public List<string> Schemas { get; set; } 
        public List<SchemaTable> Tables
        {
            get
            {
                if (!_tablesAreOrdered)
                {
                    GetOrder();
                    _tablesAreOrdered = true;
                }
                return _tables;
            }

            private set => _tables = value; 
        }

        private bool _tablesAreOrdered;

        public SchemaDatabase()
        {
            Tables = new List<SchemaTable>();
            Sequences = new List<SchemaSequence>();
            Schemas = new List<string>();
        }
        public SchemaDatabase(string databaseName, List<SchemaTable> tables, List<SchemaSequence> sequences, List<string> schemas)
        {
            DatabaseName = databaseName;
            Tables = tables;
            Schemas = schemas;
            Sequences = sequences;
        }
        public List<SchemaSequence> Sequences { get; private set; }
        internal void AddSequence(SchemaSequence sequence) => Sequences.Add(sequence);
        internal void AddSequences(IEnumerable<SchemaSequence> sequences)
        {
            if (Sequences == null)
            {
                Sequences = sequences.ToList();
                return;
            }
            if (sequences != null) Sequences.AddRange(sequences);
        }

        public void AddSchema(string schemaName) => Schemas.Add(schemaName);
        public void AddSchemas(IEnumerable<string> schemaNames) => Schemas.AddRange(schemaNames);
        
        public void AddTable(SchemaTable table)
        {
            _tables.Add(table);
            _tablesAreOrdered = false;
        }
        public void AddTables(IEnumerable<SchemaTable> tables)
        {
            if (tables == null) return;
            this._tables.AddRange(tables);
            _tablesAreOrdered = false;
        }
        private void GetReferencedTables()
        {
            foreach (var table in _tables)
            {
                foreach (var fk in table.ForeignKeys)
                {
                    if (fk.IsSelfReference) continue;
                    else
                    {
                        var referencedTable = _tables.FirstOrDefault(t => t.TableName == fk.ReferencedTable);
                        if (table.ReferencedTables.Contains(referencedTable)) continue;
                        table.ReferencedTables.Add(referencedTable);
                    }
                }
            }
        }
        private void GetOrder()
        {
            GetReferencedTables();
            List<SchemaTable> orderedTables = new List<SchemaTable>();

            foreach (var table in _tables)
            {
                Func(table);
            }

            _tables = orderedTables;

            void Func(SchemaTable table)
            {
                if (table.IsVisited) return;
                foreach (var referencedTable in table.ReferencedTables)
                {
                    Func(referencedTable);
                }
                table.IsVisited = true;
                orderedTables.Add(table);
            }
        }
    }
}
