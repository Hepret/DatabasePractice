using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.TableDataComponents;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.ScriptCreators
{
    class CreatorScriptsForInsertDataMSSQLToMSSQL
    {
        public static DataInsertScripts[] CreateInsertDataScript(DatabaseData data)
        {
            var ammountOfTable = data.DatabaseShchema.Tables.Count;
            DataInsertScripts[] insertScripts = new DataInsertScripts[ammountOfTable];
            for (int i = 0; i < ammountOfTable; i++)
            {
                insertScripts[i] = CreateInsertDataScriptIntoTable(data[i]);
            }
            return insertScripts;
        }
        private static DataInsertScripts CreateInsertDataScriptIntoTable(TableData data)
        {
            DataInsertScripts insertScripts = new DataInsertScripts();
            if (data.AmmountOfRows == 0) return insertScripts;
            var tableSchema = data.TableSchema;
            foreach (var dataInterval in data.Data)
            {
                var AddDataIntervalScript = CreateInsertDataIntervalIntoTableScript(tableSchema, dataInterval);
                insertScripts.Add(AddDataIntervalScript);
            }

            return insertScripts;
        }
        private static string CreateInsertDataIntervalIntoTableScript(SchemaTable table, TableDataRows dataForInsert)
        {

            string tableName = table.TableName;
            StringBuilder InsertString = new StringBuilder();

            InsertString.AppendLine($"INSERT INTO [{table.SchemaCatalog}].[{tableName}] ({ChoiceColumsWithoutIdentityAndGenerated(table)})" +
                $"\nVALUES");



            string[] stringRows = new string[dataForInsert.Count];

            for (int i = 0; i < dataForInsert.Count; i++)
            {
                var currentRow = dataForInsert[i];
                stringRows[i] = CreateRowToInsertString(currentRow, table);
            }

            var AllRowsString = string.Join(",\n", stringRows);

            InsertString.AppendLine(AllRowsString);
            InsertString.AppendLine(";");

            return InsertString.ToString();
        }

        private static string ChoiceColumsWithoutIdentityAndGenerated(SchemaTable table)
        {
            List<string> columnNames = new List<string>();
            foreach (var column in table.Columns)
            {
                if (column.Is_identity != "True" && column.Is_generated != "1") columnNames.Add($"[{column.Column_name}]");
            }
            var columnsWithoutIdentity = string.Join(",", columnNames);
            return columnsWithoutIdentity;
        }

        private static string CreateRowToInsertString(TableDataRow row, SchemaTable table)
        {
            List<string> stringRow = new List<string>();
            for (int i = 0; i < row.ColumnAmmount; i++)
            {
                if (table.Columns[i].Is_identity == "True" || table.Columns[i].Is_generated == "1") continue;
                var itemString = CreateItemString(row[i], table.Columns[i]);
                stringRow.Add(itemString);
            }
            string InsertRowString = "(" + string.Join(", ", stringRow) + ")";
            return InsertRowString;
        }
        private static string CreateItemString(object item, SchemaColumn column_info)
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
                case DateTime date when column_info.Data_type == "date":
                    return $"'{date.Year}-{date.Month}-{date.Day}'";
                case DateTime date when column_info.Data_type.ToLower() == "datetime2" || column_info.Data_type.ToLower() == "datetime":
                    return $"'" +
                        $"{date.Year}-{date.Month}-{date.Day} " +
                        $"{date.Hour}:{date.Minute}:{date.Second}:{date.Millisecond}'";
                case DateTimeOffset date when column_info.Data_type == "datetimeoffset":
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

