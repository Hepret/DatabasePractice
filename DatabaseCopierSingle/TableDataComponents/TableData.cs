using DatabaseCopierSingle.DatabaseTableComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle
{
   
    class TableData : IEnumerable
    {
        public List<TableDataRows> Data { get; private set; }
        public SchemaTable TableSchema { get; private set; }
        public int AmmountOfRows
        {
            get
            {
                int res = 0;
                foreach (var dataRows in Data)
                {
                    res += dataRows.Count;
                }
                return res;
            }
        }
        public TableDataRows this[int index]
        {
            get => Data[index];
        }
        public TableData(SchemaTable schemaTable)
        {
            TableSchema = schemaTable;
            Data = new List<TableDataRows>();
        }
        public void AddData(TableDataRows tableDataRows)
        {
            Data.Add(tableDataRows);
        }
        public void AddData(IEnumerable<TableDataRows> dataIntervals)
        {
            Data.AddRange(dataIntervals);
        }
        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }
    
}
