using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertData;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptsCreatorForInsertingDatabaseData
{
    class CreatorScriptsesForInsertDataPostgresqlToPostgresql : ICreateInsertDataScripts
    {
        public DataInsertScripts CreateInsertDataScript(DatabaseData data)
        {
            var amountOfTable = data.TableData.Length;
            DataInsertScripts scripts = new DataInsertScripts(data.DatabaseSchema);

            for (int i = 0; i < amountOfTable; i++)
            {
                var tableDataInsertScripts = CreateInsertDataScriptIntoTable(data[i]);
                scripts.AddTableDataInsertScripts(i, tableDataInsertScripts);
            }

            return scripts;
        }
        private string[] CreateInsertDataScriptIntoTable(TableData data)
        {
            string[] tableDataInsertScripts = new string[data.Data.Count];
            for (int i = 0; i < data.Data.Count; i++)
            {
                tableDataInsertScripts[i] = CreateInsertDataIntervalIntoTableScript(data.TableSchema, data[i]);
            }

            return tableDataInsertScripts;
        }
        private string CreateInsertDataIntervalIntoTableScript(SchemaTable table, DataRowInterval dataForInsert)
        {
            
            string tableName = table.TableName;
            StringBuilder insertString = new StringBuilder();
            
            insertString.AppendLine($"INSERT INTO \"{table.SchemaCatalog}\".\"{tableName}\" ({ChoiceColumnsWithoutIdentityAndGenerated(table)})" +
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

        private string ChoiceColumnsWithoutIdentityAndGenerated(SchemaTable table)
        {
            List<string> columnNames = new List<string>();
            foreach (var column in table.Columns)
            {
                if (column.Identity_generation != "ALWAYS" && column.Is_generated != "ALWAYS") columnNames.Add($"\"{column.Column_name}\"");
            }
            var columnsWithoutIdentity = string.Join(",", columnNames);
            return columnsWithoutIdentity;
        }

        private string CreateRowToInsertString(TableDataRow row, SchemaTable table)
        {
            List<string> stringRow = new List<string>();
            for (int i = 0; i < row.ColumnAmount; i++)
            {
                if (table.Columns[i].Identity_generation =="ALWAYS" || table.Columns[i].Is_generated == "ALWAYS") continue;
                var itemString = CreateItemString(row[i], table.Columns[i]);
                stringRow.Add(itemString);
            }
            string insertRowString = "(" + string.Join(", ", stringRow) + ")";
            return insertRowString;
        }
        private string CreateItemString(object item, SchemaColumn columnInfo)
        {
            var specifier = "G";
            switch (item)
            {
                case string expression:
                    return $"'{expression.Replace("'", "''")}'";
                case double num:
                    return num.ToString(specifier, CultureInfo.InvariantCulture);
                case decimal num:
                    return num.ToString(specifier, CultureInfo.InvariantCulture);
                case TimeSpan time:
                    return "'" + time.ToString() + "'";
                case DateTime date when columnInfo.Data_type == "date":
                    return $"'{date.Year}-{date.Month}-{date.Day}'";
                case DateTimeOffset date when columnInfo.Data_type == "timestamp without time zone":
                    return $"'" +
                        $"{date.Year}-{date.Month}-{date.Day} " +
                        $"{date.Hour}:{date.Minute}:{date.Second} {date.Offset}'";
                case DBNull _:
                    return "null";
                default:
                    var tmp = item.ToString();
                    return $"'{ tmp.Replace("'", "''")}'";
            }
        }
    }
}
