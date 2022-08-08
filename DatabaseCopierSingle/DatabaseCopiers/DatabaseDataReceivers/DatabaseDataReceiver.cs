using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.Remoting.Proxies;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers
{
    public abstract class DatabaseDataReceiver : IDatabaseDataReceiver
    {
        protected readonly DatabaseProvider Provider;

        protected DatabaseDataReceiver(DatabaseProvider provider)
        {
            Provider = provider;
        }
        
        public DatabaseData GetDatabaseData(SchemaDatabase schemaDatabase)
        {
            DatabaseData databaseData = new DatabaseData(schemaDatabase);
            foreach (var table in databaseData.TableData)
            {
                // Empty Test
                var amountOfRowsInTheTable = GetNumberOfRowsInTheTable(table.TableSchema.FullTableName);
                if (amountOfRowsInTheTable == 0) continue;

                var tableHasSelfReference = table.TableSchema.HasSelfReference;
                var data = tableHasSelfReference ? 
                    GetDataFromTableWithSelfReference(table, amountOfRowsInTheTable)
                    : GetDataFromTable(table, amountOfRowsInTheTable);
                
                table.AddData(data);
            }
            return databaseData;
        }

        protected abstract IEnumerable<TableDataRow> GetDataFromTable(TableData table, int amountOfRows);
        
        private IEnumerable<TableDataRow> GetDataFromTableWithSelfReference(TableData table, int amountOfRows)
        {
            object[] receivedFields = null;
            string selfReferencingColumn =
                table.TableSchema.ForeignKeys.First(fk => fk.IsSelfReference).ColumnName;
            string parentColumn = 
                table.TableSchema.ForeignKeys.First(fk => fk.IsSelfReference).ReferencedColumn;

            TableDataRow[] allRowsInTable = new TableDataRow[amountOfRows];
            int counter = 0; // points to the current index in the array allRowsInTable
            while (amountOfRows > 0)
            {

                var rows = GetRowsWithSelfReferencing(receivedFields, table.TableSchema, selfReferencingColumn);
                receivedFields = rows
                    .Select(row => row[parentColumn])
                    .ToArray();
                for (var i = 0; i < rows.Length; i++)
                {
                    allRowsInTable[i + counter] = rows[i];
                }

                counter += rows.Length;
                
                amountOfRows -= receivedFields.Length;
            }

            return allRowsInTable;
        }
        protected TableDataRow GetDataRowFromReader(DbDataReader reader, SchemaTable schemaTable)
        {
            var amountOfColumns = reader.FieldCount;
            object[] tmp = new object[amountOfColumns];
            reader.GetValues(tmp);
            TableDataRow row = new TableDataRow((object[])tmp.Clone(), schemaTable);
            return row;
        }
        //protected abstract TableDataRow[] GetRangeOfRowsFromTable(SchemaTable schemaTable, int startWith = 0, int amountOfRows = 100);
        protected abstract int GetNumberOfRowsInTheTable(FullTableName tableName);
        protected abstract TableDataRow[] GetRowsWithSelfReferencing(object[] receivedFields, SchemaTable schemaTable, string selfReferencingColumn);


        protected abstract int GetAmountRowsInTableWithSelfReferencing(string receivedFieldsString, SchemaTable schemaTable, string selfReferencingColumn);

        protected int GetNumberOfRowsInTheTable(string queryString)
        {
            var amountOfRows =  Provider.ExecuteCommandScalar(queryString);
            return amountOfRows;
        }
    }
}