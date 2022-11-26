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
    public class CreatorScriptsForInsertDataPostgresqlToPostgresql : ICreateInsertDataScripts
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
            
            insertString.AppendLine($"INSERT INTO \"{table.SchemaCatalog}\".\"{tableName}\" ({ChoiceColumnsWithoutGenerated(table)})" +
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
            
            if (item is DBNull) return "null";
            
            var datatype = columnInfo.UdtName;
            
            var datatypeIsArray = datatype[0] == '_';
            
            if (datatypeIsArray)
            {
                return GetItemStringForArray(item, datatype);
            }

            else
            {
                return GetItemString(item, datatype);
            }
            
        }

        private string GetItemStringForArray(object item, string datatype)
        {
            var cellStringBuilder = new StringBuilder();
            datatype = datatype.Substring(1);
            cellStringBuilder.Append("ARRAY[");

            string[] arrayItems = new string[]{};
            
            switch (item)
            {
                case int[] arrayInts:
                    arrayItems = new string[arrayInts.Length];
                    for (int i = 0; i < arrayInts.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayInts[i], datatype);
                    }
                    break;
                case short[] arrayShorts:
                    arrayItems = new string[arrayShorts.Length];
                    for (int i = 0; i < arrayShorts.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayShorts[i], datatype);
                    }
                    break;
                case long[] arrayBigints:
                    arrayItems = new string[arrayBigints.Length];
                    for (int i = 0; i < arrayBigints.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayBigints[i], datatype);
                    }
                    break;
                case char[] arrayChars:
                    arrayItems = new string[arrayChars.Length];
                    for (int i = 0; i < arrayChars.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayChars[i], datatype);
                    }
                    break;
                case string[] arrayStrings:
                    arrayItems = new string[arrayStrings.Length];
                    for (int i = 0; i < arrayStrings.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayStrings[i], datatype);
                    }
                    break;
                
                case TimeSpan[] arrayTimeSpans:
                    arrayItems = new string[arrayTimeSpans.Length];
                    for (int i = 0; i < arrayTimeSpans.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayTimeSpans[i], datatype);
                    }
                    break;
                case DateTime[] arrayDateTimes:
                    arrayItems = new string[arrayDateTimes.Length];
                    for (int i = 0; i < arrayDateTimes.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayDateTimes[i], datatype);
                    }
                    break;
                case bool[] arrayBools:
                    arrayItems = new string[arrayBools.Length];
                    for (int i = 0; i < arrayBools.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayBools[i], datatype);
                    }
                    break;
                case double[] arrayDoubles:
                    arrayItems = new string[arrayDoubles.Length];
                    for (int i = 0; i < arrayDoubles.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayDoubles[i], datatype);
                    }
                    break;
                case float[] arrayFloats:
                    arrayItems = new string[arrayFloats.Length];
                    for (int i = 0; i < arrayFloats.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayFloats[i], datatype);
                    }
                    break;
                case decimal[] arrayDecimals:
                    arrayItems = new string[arrayDecimals.Length];
                    for (int i = 0; i < arrayDecimals.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayDecimals[i], datatype);
                    }
                    break;
                case  object[] arrayObjects:
                    arrayItems = new string[arrayObjects.Length];
                    for (int i = 0; i < arrayObjects.Length; i++)
                    {
                        arrayItems[i] = GetItemString(arrayObjects[i], datatype);
                    }
                    break;
            }

            cellStringBuilder.Append(String.Join(",",arrayItems));
            cellStringBuilder.Append("]");
            var resString = cellStringBuilder.ToString();
            return resString;
        }

        private string GetItemString(object item, string datatype)
        {
            var specifier = "G";
            switch (datatype) 
            {
                case "varchar":
                case "character varying":
                case "character":
                case "char":
                case "text": 
                    return $"'{((string)item).Replace("'", "''")}'";
                
                case "smallint":
                case "int2":
                case "int4":
                case "int8":
                case "integer":
                case "int":
                case "bigint":
                    return item.ToString();
                
                case "real":
                case "float":
                case "float8":
                case "float4":
                case "numeric":
                case "double precision":
                    return double.Parse(item.ToString()).ToString(specifier, CultureInfo.InvariantCulture);
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
                case "bool":
                    return (bool) item ? "false" : "true";
                case "timestamp with time zone":
                case "timestamp":
                case "timestamptz":
                case "timestamp without time zone":
                    var timestamp = (DateTime) item;
                    return
                        $"'" +
                        $"{timestamp.Year}-{timestamp.Month}-{timestamp.Day} " +
                        $"{timestamp.Hour}:{timestamp.Minute}:{timestamp.Second}'";
                case "bytea":
                    return $"'{item}'";
                
                default:
                    var tmp = item.ToString();
                    return $"'{ tmp.Replace("'", "''")}'";
            }
        }
    }
}
