using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseDataReceivers
{
    public class PostgresqlDataReceiver : DatabaseDataReceiver
    {
        public PostgresqlDataReceiver(DatabaseProvider provider) : base(provider)
        {
        }

        protected override IEnumerable<TableDataRow> GetDataFromTable(TableData table, int amountOfRows)
        {
            try
            {
                var queryString =
                    $"SELECT *\n" +
                    $"FROM \"{table.TableSchema.SchemaCatalog}\".\"{table.TableSchema.TableName}\"\n";

                var dataRows = new TableDataRow[amountOfRows];
                using (var reader = Provider.GetDataReader(queryString))
                {
                    int ch = 0; // counter 
                    while (reader.Read())
                    {
                        dataRows[ch] = GetDataRowFromReader(reader,table.TableSchema);
                        ch++;
                    }
                }
                
                
                return dataRows;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get data from table: {table.TableSchema.SchemaCatalog}.{table.TableSchema.TableName}",ex);
            }

        }

        protected override int GetNumberOfRowsInTheTable(FullTableName tableName)
        {
            var queryString =
                $"SELECT COUNT(*)\n" +
                $"FROM \"{tableName.SchemaCatalogName}\".\"{tableName.TableName}\"";
            var rowsAmount = Provider.ExecuteCommandScalar(queryString);
            return rowsAmount;
        }

        protected override TableDataRow[] GetRowsWithSelfReferencing(object[] receivedFields, SchemaTable schemaTable,
            string selfReferencingColumn)
        {
            
            var receivedFieldsString = receivedFields == null ? 
                "IS NULL" :
                $"IN ({string.Join(",", receivedFields.Select(f => f.ToString()))})";

            var amountOfRows = GetAmountRowsInTableWithSelfReferencing(receivedFieldsString, schemaTable, selfReferencingColumn);
            TableDataRow[] rows = new TableDataRow[amountOfRows];
            
            var queryString =
                "SELECT *\n" +
                $"FROM \"{schemaTable.SchemaCatalog}\".\"{schemaTable.TableName}\"\n" +
                $"WHERE {selfReferencingColumn} {receivedFieldsString}";

            using (var reader = Provider.GetDataReader(queryString))
            {
                for (int i = 0; i < amountOfRows; i++)
                {
                    reader.Read();
                    rows[i] = GetDataRowFromReader(reader, schemaTable);
                }
                return rows;
            }

        }

        protected override int GetAmountRowsInTableWithSelfReferencing(string receivedFieldsString, SchemaTable schemaTable,
            string selfReferencingColumn)
        {
            // First invoke 
            var queryString =
                "SELECT  COUNT(*)\n" +
                $"FROM \"{schemaTable.SchemaCatalog}\".\"{schemaTable.TableName}\"\n" +
                $"WHERE {selfReferencingColumn} {receivedFieldsString}";
            
            var amountOfRows = GetNumberOfRowsInTheTable(queryString);

            return amountOfRows;
        }

        
    }
}