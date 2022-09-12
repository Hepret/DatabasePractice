using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.ScriptCreators
{
    class TypesFromPostgresqlToMssql
    {
        public static string Get (string dataType)
        {
            switch (dataType.ToLower())
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
                    return "time"; 
                case "timestamp":
                    return "datetime";
                case "timestamptz":
                case "timestamp with time zone":
                    return "datetimeoffset";
                case "boolean":
                    return "bit"; 
                case "xml":
                    return "xml";
                
                default:
                    return dataType;
            }
        }
    }
}
