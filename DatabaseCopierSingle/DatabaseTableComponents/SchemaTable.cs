using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class SchemaTable
    {
       
        public List<CheckConstraint> CheckConstraints { get; private set; } 
        public List<UniqueConstraint> UniqueConstraints { get; private set; }
        public PrimaryKey PrimaryKey { get; set; }
        public List<SchemaColumn> Columns { get; private set; }
        public List<ForeignKey> ForeignKeys { get; private set; }
        public string TableName { get; set; }
        public string SchemaCatalog { get; set; }
        public List<SchemaTable> ReferencedTables { get; set; } = new List<SchemaTable>();
        public bool SelfReference { get; set; }
        public SchemaTable(string tableName)
        {
            TableName = tableName;
            Columns = new List<SchemaColumn>();
            ForeignKeys = new List<ForeignKey>();
            UniqueConstraints = new List<UniqueConstraint>();
            CheckConstraints = new List<CheckConstraint>();
        }
        public FullTableName FullTableName { get => 
                new FullTableName
                {
                    TableName = this.TableName,
                    SchemaCatalogName = this.SchemaCatalog
                };
             }
        public SchemaTable(string tableName, List<SchemaColumn> columns, List<ForeignKey> foreignKeys, PrimaryKey primaryKey, List<UniqueConstraint> uniqueConstraints, List<CheckConstraint> checkConstraints, string schemaCatalog)
        {
            SchemaCatalog = schemaCatalog;
            TableName = tableName;
            Columns = columns;
            ForeignKeys = foreignKeys;
            PrimaryKey = primaryKey;
            UniqueConstraints = uniqueConstraints;
            CheckConstraints = checkConstraints;
        }
        

        internal bool isVisited = false;

        internal void AddUnique(UniqueConstraint unique) => UniqueConstraints.Add(unique);
        internal void AddUnique(IEnumerable<UniqueConstraint> uniques)
        {
            if (uniques != null) this.UniqueConstraints.AddRange(uniques);
        }
        internal void AddCheckConstraint(CheckConstraint checkConstraint)
        {
            CheckConstraints.Add(checkConstraint);
        }
        internal void AddCheckConstraint(IEnumerable<CheckConstraint> checkConstraints)
        {
            CheckConstraints.AddRange(checkConstraints);
        }
        internal void SelfReferenceCheck()
        {
            foreach (var fk in ForeignKeys)
            {
                if (fk.ReferencedTable == this.TableName)
                {
                    SelfReference = true;
                    break;
                }
            }
        }

        public void AddForeignKey(ForeignKey foreignKey) => ForeignKeys.Add(foreignKey);
        public void AddForeignKey(IEnumerable<ForeignKey> foreignKeys)
        {
            if (foreignKeys != null) this.ForeignKeys.AddRange(foreignKeys);
        }
        public void AddColumn(SchemaColumn column) => Columns.Add(column);
        public void AddColumn(List<SchemaColumn> columns)
        {
            if (columns != null) this.Columns.AddRange(columns);
        }

    }
}
