using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.ScriptCreators
{
    class TypesFromMssqltoPostgresql
    {
        public static string GetDataTypeFromPostgresqlToMssql(string dataType)
        {
            switch (dataType)
            {
                case "smallint":
                    return "smallint";
                case "integer":
                    return "int";
                case "bigint":
                    return "bigint";
                case "decimal":
                    return "decimal";
                case "numeric":
                    return "numeric";
                case "real":
                    return "real";
                case "double precision":
                    return "float";
                case "money":
                    return "money";
                case "character varying":
                case "varchar":
                    return "nvarchar";
                case "character":
                case "char":
                   return "nchar";
                case "text":
                    return "ntext";
                case "bytea":
                    return "binary";
                case "date":
                    return "date";
                case "time without time zone":
                case "time":
                    return "time";
                case "time with time zone":
                    return "time"; // TODO transform
                case "timestamp":
                    return "datetime";
                case "timestamptz":
                case "timestamp with time zone":
                    return "datetimeoffset";
                case "boolean":
                    return "bit"; // TODO transform
                case "xml":
                    return "xml";

                // interval not included
            }
            throw new ArgumentException($"This data type: {dataType} - doesn't exist");
        }
    }
}
