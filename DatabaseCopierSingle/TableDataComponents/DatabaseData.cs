using DatabaseCopierSingle.DatabaseTableComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.TableDataComponents
{
    public class DatabaseData : IEnumerable
    {
        public TableData[] TableData { get; private set; }
        public SchemaDatabase DatabaseSchema { get; private set; }
        public DatabaseData(SchemaDatabase schemaDatabase)
        {
            DatabaseSchema = schemaDatabase;
            var amountOfTables = schemaDatabase.Tables.Count;
            TableData = new TableData[amountOfTables];
            for (var i = 0; i < amountOfTables; i++)
            {
                var tableSchema = schemaDatabase.Tables[i];
                TableData[i] = new TableData(tableSchema);
            }

        }


        public void AddDataToTable(int tableIndex, List<DataRowInterval> dataIntervals)
        {
            try
            {
                var table = TableData[tableIndex];
                AddDataToTable(table, dataIntervals);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException($"Can't add data to table, because table with index: {tableIndex} - doesn't exist", ex);

            }
        }
        public void AddDataToTable(int tableIndex, DataRowInterval rowInterval)
        {
            try
            {
                TableData[tableIndex].AddData(rowInterval);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ArgumentOutOfRangeException($"Can't add data to table, because table with index: {tableIndex} - doesn't exist", ex); 
            }
        }
        public void AddDataToTable(string tableName, List<DataRowInterval> dataIntervals)
        {
            var table = Array.Find(TableData, t => t.TableSchema.TableName == tableName);
            if (table == null)
            {
                throw new ArgumentException($"Can't add data to table: {tableName}, because it doesn't exist");
            }

            AddDataToTable(table, dataIntervals);
        }
        public void AddDataToTable(string tableName, DataRowInterval rowInterval)
        {
            
            var table = Array.Find(TableData, t => t.TableSchema.TableName == tableName);
            if (table == null)
            {
                throw new ArgumentException($"Can't add data to table: {tableName}, because it doesn't exist");
            }
            AddDataToTable(table, rowInterval);
        }
        public void AddDataToTable(TableData table, DataRowInterval rowInterval)
        {
            table.AddData(rowInterval);
        }
        public void AddDataToTable(TableData table, List<DataRowInterval> dataIntervals)
        {
            table.AddData(dataIntervals);
        }
        public TableData this[int index] => TableData[index];

        public IEnumerator GetEnumerator()
        {
            return TableData.GetEnumerator();
        }
    }
}
