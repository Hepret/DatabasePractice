using System.Collections;
using System.Linq;
using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.TableDataComponents
{
    public class TableDataRow : IEnumerable
    {
        private object[] Data { get; set; } // Data from db row
        private SchemaTable SchemaTable { get; set; }
        public int ColumnAmount => Data.Length;

        public object this[string columnName] =>
            Data[SchemaTable.Columns.Where(column => column.Column_name == columnName)
                .Select(column => int.Parse(column.Ordinal_position)-1)
                .First()];
        public object this[int index] => Data[index];

        public TableDataRow(object[] data, SchemaTable schema)
        {
            Data = data;
            SchemaTable = schema;
        }
        public IEnumerator GetEnumerator() => Data.GetEnumerator();
    }
}