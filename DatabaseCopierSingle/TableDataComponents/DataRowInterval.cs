using System;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseCopierSingle.TableDataComponents
{
    public class DataRowInterval
    {
        private List<TableDataRow> DataInterval { get;  set; } = new List<TableDataRow>();
        public const int MaxRows = 100;
        
        public int Count => DataInterval.Count;

        public TableDataRow this[int index] => DataInterval[index];

        
        public void AddRow(TableDataRow row)
        {
            if (DataInterval.Count + 1 == MaxRows) throw new Exception($"Already maximum element ({MaxRows} rows)");
            DataInterval.Add(row);
        }

        public void AddRow(IEnumerable<TableDataRow> rows)
        {
            var tableDataRows = rows.ToList();
            if (DataInterval.Count + tableDataRows.Count() > MaxRows) 
                throw new Exception($"Can't add elements. Max rows: {MaxRows}, already has: {DataInterval.Count}, want to add: {tableDataRows.Count()}");
            DataInterval.AddRange(tableDataRows);
            
        }
    }
}
