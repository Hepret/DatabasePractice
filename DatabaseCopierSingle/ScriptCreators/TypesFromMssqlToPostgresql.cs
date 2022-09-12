using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.ScriptCreators
{
    public static class TypesFromMssqlToPostgresql
    {
        public static string Get(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "binary":
                case "varbinary":
                case "rowversion":
                case "image":
                case "fieldhierarchyid":
                    return "bytea";
                
                case "bit":
                    return "boolean";
                
                case "nchar":
                    return "char";
                
                case "varchar":
                case "nvarchar":
                    return "varchar";

                case "ntext":
                    return "text";
                
                
                case "money":
                case "smallmoney":
                    return "money";
                
                case "tinyint":
                    return "smallint";
                
                case "uniqueidentifier":
                    return "uuid";
                
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return "timestamp";
                
                case "datetimeoffset":
                    return "timestamptz";
                
                case "float":
                    return "double precision";

                default:
                    return dataType;

            }
            
           
        }
    }
}