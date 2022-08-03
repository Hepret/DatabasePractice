using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class SchemaDatabase
    {

        List<SchemaTable> tables;
        public string DatabaseName { get; set; }
        public List<string> Schemas { get; set; } 
        public List<SchemaTable> Tables
        {
            get
            {
                if (!isOdered)
                {
                    GetOrder();
                    isOdered = true;
                }
                return tables;
            }

            private set => tables = value; 
        }

        bool isOdered = false;

        public SchemaDatabase()
        {
            Tables = new List<SchemaTable>();
            Sequences = new List<SchemaSequence>();
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
        
        public void AddTable(SchemaTable table)
        {
            tables.Add(table);
            isOdered = false;
        }
        public void AddTables(IEnumerable<SchemaTable> tables)
        {
            if (tables == null) return;
            this.tables.AddRange(tables);
            isOdered = false;
        }
        private void GetReferencedTables()
        {
            foreach (var table in tables)
            {
                foreach (var fk in table.ForeignKeys)
                {
                    if (fk.IsSelfReference) continue;
                    else
                    {
                        var referencedTable = tables.Where(t => t.TableName == fk.ReferencedTable).FirstOrDefault();
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

            foreach (var table in tables)
            {
                Func(table);
            }

            tables = orderedTables;

            void Func(SchemaTable table)
            {
                if (table.isVisited) return;
                foreach (var referencedTable in table.ReferencedTables)
                {
                    Func(referencedTable);
                }
                table.isVisited = true;
                orderedTables.Add(table);
            }
        }
        
    }
}
