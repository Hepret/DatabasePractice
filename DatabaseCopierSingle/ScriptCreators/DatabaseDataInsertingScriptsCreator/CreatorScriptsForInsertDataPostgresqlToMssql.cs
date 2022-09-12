using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertData;
using DatabaseCopierSingle.ScriptCreators.ScriptsCreatorForInsertingDatabaseData;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle.ScriptCreators.DatabaseDataInsertingScriptsCreator
{
    public class CreatorScriptsForInsertDataPostgresqlToMssql : ICreateInsertDataScripts
    {
        public DataInsertScripts CreateInsertDataScript(DatabaseData data)
        {
            var amountOfTable = data.TableData.Length;
            var scripts = new DataInsertScripts(data.DatabaseSchema);

            for (var i = 0; i < amountOfTable; i++)
            {
                var tableDataInsertScripts = CreateInsertDataScriptIntoTable(data[i]);
                scripts.AddTableDataInsertScripts(i, tableDataInsertScripts);
            }

            return scripts;
        }
        
        private string[] CreateInsertDataScriptIntoTable(TableData data)
        {
            var tableDataInsertScripts = new string[data.Data.Count];
            for (var i = 0; i < data.Data.Count; i++)
            {
                tableDataInsertScripts[i] = CreateInsertDataIntervalIntoTableScript(data.TableSchema, data[i]);
            }

            return tableDataInsertScripts;
        }
        
        private string CreateInsertDataIntervalIntoTableScript(SchemaTable table, DataRowInterval dataForInsert)
        {
            string tableName = table.TableName;
            StringBuilder insertString = new StringBuilder();
            var schemaCatalog = table.SchemaCatalog == "public" ? "dbo" : table.SchemaCatalog;
            insertString.AppendLine(
                $"SET IDENTITY_INSERT [{schemaCatalog}].[{tableName}] ON;\n" +
                $"INSERT INTO [{schemaCatalog}].[{tableName}] ({ChoiceColumnsWithoutGenerated(table)})" +
                $"\nVALUES");

            string[] stringRows = new string[dataForInsert.Count];

            for (int i = 0; i < dataForInsert.Count; i++)
            {
                var currentRow = dataForInsert[i];
                stringRows[i] = CreateRowToInsertString(currentRow, table);
            }

            var allRowsString = string.Join(",\n", stringRows);

            insertString.AppendLine(allRowsString);
            insertString.AppendLine(";");
            insertString.AppendLine($"SET IDENTITY_INSERT [{schemaCatalog}].[{tableName}] OFF;\n");
            return insertString.ToString();
        }

        private string ChoiceColumnsWithoutGenerated(SchemaTable table)
        {
            var columnNames = table.Columns
                .Where(col => col.IsGenerated != "ALWAYS")
                .Select(col => col.ColumnName);
            var columnsWithoutIdentity = string.Join(",", columnNames);
            return columnsWithoutIdentity;
        }
        
        private string CreateRowToInsertString(TableDataRow row, SchemaTable table)
        {
            var stringRow = new List<string>();
            for (var i = 0; i < row.ColumnAmount; i++)
            {
                if (table.Columns[i].IsGenerated == "ALWAYS") continue;
                var itemString = CreateItemString(row[i], table.Columns[i]);
                stringRow.Add(itemString);
            }
            var insertRowString = "(" + string.Join(", ", stringRow) + ")";
            return insertRowString;
        }
        
        private string CreateItemString(object item, SchemaColumn columnInfo)
        {
            var specifier = "G";
            if (item is DBNull) return "null";
            switch (columnInfo.DataType)
            {
                case "char":
                case "varchar":
                case "text":
                case "nchar":
                case "nvarchar":
                case "ntext":
                    return $"'{item.ToString().Replace("'", "''")}'";
                
                case "bit":
                    return bool.Parse(item.ToString()) ? "1" : "0";
                
                case "bigint":
                case "int":
                case "smallint":
                    return item.ToString();
                
                case "numeric":
                case "decimal":
                case "money":
                case "smallmoney":
                    return((decimal) item).ToString(specifier, CultureInfo.InvariantCulture);
                case "float":
                case "real":
                    return((double) item).ToString(specifier, CultureInfo.InvariantCulture);
                
                case "date":
                    var date = (DateTime) item;
                    return $"'{date.Year}-{date.Month}-{date.Day}'";
                case "smalldatetime":   
                case "datetime2":
                case "datetime":
                    var dateTime = (DateTime) item;
                    return $"'" +
                           $"{dateTime.Year}-{dateTime.Month}-{dateTime.Day} " +
                           $"{dateTime.Hour}:{dateTime.Minute}:{dateTime.Second}:{dateTime.Millisecond}'";
                
                case "datetimeoffset":
                    var dateTimeOffset = (DateTimeOffset) item;
                    return $"'" +
                           $"{dateTimeOffset.Year}-{dateTimeOffset.Month}-{dateTimeOffset.Day} " +
                           $"{dateTimeOffset.Hour}:{dateTimeOffset.Minute}:{dateTimeOffset.Second} {dateTimeOffset.Offset}'";
                case "time":
                    var timeSpan = (TimeSpan) item;
                    return $"'" +
                           $"{timeSpan.Hours}:{timeSpan.Minutes}:{timeSpan.Seconds}:{timeSpan.Milliseconds}'";

                default:
                    var tmp = item.ToString();
                    return $"'{tmp.Replace("'", "''")}'";

            }
        }
    }
}