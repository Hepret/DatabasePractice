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
    public class CreatorScriptsForInsertDataMssqlToPostgresql :  ICreateInsertDataScripts
    
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
            
            string schemaCatalog = table.SchemaCatalog == "dbo" ? "public" : table.SchemaCatalog;
            insertString.AppendLine(
                $"INSERT INTO \"{schemaCatalog}\".\"{tableName}\" ({ChoiceColumnsWithoutGenerated(table)})" +
                                    "OVERRIDING SYSTEM VALUE \n" + 
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

            return insertString.ToString();
        }

        private string ChoiceColumnsWithoutGenerated(SchemaTable table)
        {
            var columnNames = table.Columns
                .Where(col => col.IsGenerated != "1")
                .Select(col => $"\"{col.ColumnName}\"");
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

            var datatype = TypesFromMssqlToPostgresql.Get(columnInfo.DataType);
            switch (datatype) 
            {
                case "varchar":
                case "character varying":
                case "character":
                case "char":
                case "text": 
                    return $"'{((string)item).Replace("'", "''")}'";
                
                case "smallint":
                case "integer":
                case "int":
                case "bigint":
                    return item.ToString();
                
                case "real":
                case "numeric":
                case "double precision":
                    return ((double)item).ToString(specifier, CultureInfo.InvariantCulture);
                case "decimal":
                    return ((decimal)item).ToString(specifier, CultureInfo.InvariantCulture);
                case "time":
                    return "'" + ((TimeSpan)item).ToString() + "'";
                case "date":
                {
                    var date = (DateTime) item;
                    return $"'{date.Year}-{date.Month}-{date.Day}'";
                }
                case "boolean":
                    return ((int) item) == 0 ? "false" : "true";
                case "timestamp with time zone":
                case "timestamptz":
                    var dateWithTimeZone = (DateTimeOffset) item;
                    return $"'" +
                           $"{dateWithTimeZone.Year}-{dateWithTimeZone.Month}-{dateWithTimeZone.Day} " +
                           $"{dateWithTimeZone.Hour}:{dateWithTimeZone.Minute}:{dateWithTimeZone.Second}:{dateWithTimeZone.Millisecond} {dateWithTimeZone.Offset}'";
                case "timestamp without time zone":
                    var timestamp = (DateTime) item;
                    return
                        $"'" +
                        $"{timestamp.Year}-{timestamp.Month}-{timestamp.Day} " +
                        $"{timestamp.Hour}:{timestamp.Minute}:{timestamp.Second}:{timestamp.Millisecond}";
                case "bytea":
                    return $"'{item}'";
                
                default:
                    var tmp = item.ToString();
                    return $"'{ tmp.Replace("'", "''")}'";
                
            }
        }
    }
}