using System.Collections.Generic;

namespace DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents
{
    public class PrimaryKey 
    {
        public string ConstraintName { get; set; }
        public FullTableName FullTableName { get; set; }
        public List<string> ColumnNames { get; set; }

        public void AddColumnName(string columnName)
        {
            ColumnNames.Add(columnName);
        }

        public void AddColumnNames(IEnumerable<string> columnNames)
        {
            ColumnNames.AddRange(columnNames);
        }
    }
}
