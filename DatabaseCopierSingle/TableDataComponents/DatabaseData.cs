using DatabaseCopierSingle.DatabaseTableComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.TableDataComponents
{
    class DatabaseData : IEnumerable
    {
        public TableData[] TableDatas { get; private set; }
        public SchemaDatabase DatabaseShchema { get; private set; }
        public DatabaseData(SchemaDatabase schemaDatabase)
        {
            DatabaseShchema = schemaDatabase;
            var ammountOfTables = schemaDatabase.Tables.Count;
            TableDatas = new TableData[ammountOfTables];
            for (int i = 0; i < ammountOfTables; i++)
            {
                var tableSchema = schemaDatabase.Tables[i];
                TableDatas[i] = new TableData(tableSchema);
            }

        }


        public void AddDataToTable(int tableIndex, List<TableDataRows> dataIntervals)
        {
            try
            {
                var table = TableDatas[tableIndex];
                AddDataToTable(table, dataIntervals);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException($"Can't add data to table, because table with index: {tableIndex} - doesn't exist", ex);

            }
        }
        public void AddDataToTable(int tableIndex, TableDataRows rows)
        {
            try
            {
                TableDatas[tableIndex].AddData(rows);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException($"Can't add data to table, because table with index: {tableIndex} - doesn't exist", ex); 
            }
        }
        public void AddDataToTable(string tableName, List<TableDataRows> dataIntervals)
        {
            var table = Array.Find(TableDatas, t => t.TableSchema.TableName == tableName);
            if (table == null)
            {
                throw new ArgumentException($"Can't add data to table: {tableName}, because it doesn't exist");
            }

            AddDataToTable(table, dataIntervals);
        }
        public void AddDataToTable(string tableName, TableDataRows rows)
        {
            
            var table = Array.Find(TableDatas, t => t.TableSchema.TableName == tableName);
            if (table == null)
            {
                throw new ArgumentException($"Can't add data to table: {tableName}, because it doesn't exist");
            }
            AddDataToTable(table, rows);
        }
        public void AddDataToTable(TableData table, TableDataRows rows)
        {
            table.AddData(rows);
        }
        public void AddDataToTable(TableData table, List<TableDataRows> dataIntervals)
        {
            table.AddData(dataIntervals);
        }
        public TableData this[int index]
        {
            get => TableDatas[index];
        }
        public IEnumerator GetEnumerator()
        {
            return TableDatas.GetEnumerator();
        }
    }
}
