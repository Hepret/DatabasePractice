using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.TableDataComponents;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniqueConstraint = DatabaseCopierSingle.DatabaseTableComponents.UniqueConstraint;

namespace DatabaseCopierSingle.DatabaseProviders
{
    class PostgresqlProvider : DatabaseProvider
    {
        public PostgresqlProvider(string connectionString) : base(new NpgsqlConnection(connectionString)) { }
        protected override List<CheckConstraint> GetCheckConstraints(FullTableName tableName)
        {
            List<CheckConstraint> checkConstraints = new List<CheckConstraint>();
            var queryString = "SELECT TABLE_NAME, TC.CONSTRAINT_NAME, CHECK_CLAUSE\n" +
                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC\n" +
                "INNER JOIN INFORMATION_SCHEMA.CHECK_CONSTRAINTS AS CC\n" +
                "ON TC.CONSTRAINT_NAME = CC.CONSTRAINT_NAME\n" +
                $"WHERE TABLE_NAME = '{tableName.TableName}'\n" +
                $"AND TABLE_SCHEMA = '{tableName.SchemaCatalogName}'";

            try
            {
                using (var reader = GetDataReader(queryString))
                {

                    while (reader.Read())
                    {
                        var constraintName = reader[1].ToString();
                        var checkClause = reader[2].ToString();
                        if (constraintName.Contains("not_null") || checkClause.Contains("NOT VALID")) continue;
                        checkConstraints.Add(
                            new CheckConstraint
                            {
                                TableName = reader[0].ToString(),
                                ConstraintName = constraintName,
                                CheckClause = checkClause
                            }) ;
                    }
                }
                return checkConstraints;
            }
            catch (Exception ex)
            { 
                throw new Exception($"Can't get CHECK CONSTRAINTS for table: {tableName}", ex); 
            }
        }
        protected override List<ForeignKey> GetForeignKeys(FullTableName tableName)
        {
            try
            {

                List<ForeignKey> foreignKeys = new List<ForeignKey>();
                var queryString =
                    "SELECT\n" +
                    "   tc.constraint_name,\n" +                       
                    "   tc.table_schema,\n" +                       
                    "   tc.table_name,\n" +
                    "   kcu.column_name,\n" +
                    "   ccu.table_schema as ReferesedSchema,\n" +                       
                    "   ccu.table_name AS foreign_table_name,\n" +  
                    "   ccu.column_name AS foreign_column_name\n" + 
                    "FROM\n" +
                    "   information_schema.table_constraints AS tc\n" +
                    "JOIN information_schema.key_column_usage AS kcu\n" +
                    "ON tc.constraint_name = kcu.constraint_name\n" +
                    "   AND tc.table_schema = kcu.table_schema\n" +
                    "JOIN information_schema.constraint_column_usage AS ccu\n" +
                    "ON ccu.constraint_name = tc.constraint_name\n" +
                    "   AND ccu.table_schema = tc.table_schema\n" +
                    "WHERE tc.table_schema != 'pg_catalog'\n " +
                    "   and tc.constraint_type = 'FOREIGN KEY'\n" +
                    $"  and tc.table_name = '{tableName.TableName}'\n" +
                    $" and tc.table_schema = '{tableName.SchemaCatalogName}'\n";

                using (var reader = GetDataReader(queryString))
                {

                    while (reader.Read())
                    {
                        foreignKeys.Add(
                            new ForeignKey
                            {
                                ConstraintName = reader[0].ToString(),
                                SchemaCatalog = reader[1].ToString(),
                                TableName = reader[2].ToString(),
                                ColumnName = reader[3].ToString(),
                                ReferencedSchema = reader[4].ToString(),
                                ReferencedTable = reader[5].ToString(),
                                ReferencedColumn = reader[6].ToString(),
                            }) ;
                    }
                    return foreignKeys;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get foreign keys for table: {tableName}", ex);
            }
            
            }
        protected override int GetNumberOfRowsInTheTable(FullTableName tableName)
        {
            var quaryString =
                    $"SELECT COUNT(*)\n" +
                    $"FROM \"{tableName.SchemaCatalogName}\".\"{tableName.TableName}\"";
            var RowsAmmount = ExecuteCommandScalar(quaryString);
            return RowsAmmount;
        }

        protected override PrimaryKey GetPrimaryKey(FullTableName tableName)
        {
            try
            {
                string queryString =
                        "select \n" +
                        "    tc.table_name,\n" +
                        "    kc.column_name,\n" +
                        "    tc.constraint_name\n" +
                        "from information_schema.table_constraints tc\n" +
                        "join information_schema.key_column_usage kc\n" +
                        "    on kc.table_name = tc.table_name and kc.table_schema = tc.table_schema and kc.constraint_name = tc.constraint_name\n" +
                        $"where tc.constraint_type = 'PRIMARY KEY' and tc.table_schema = '{tableName.SchemaCatalogName}' and tc.table_name = '{tableName.TableName}'\n" +
                        "            and kc.ordinal_position is not null\n" +
                        "order by tc.table_schema,\n" +
                        "         tc.table_name,\n" +
                        "         kc.position_in_unique_constraint;\n";

                using (var reader = GetDataReader(queryString))
                {
                    List<string> primaryKeyColumns = new List<string>();
                    string constraint_name = "";
                    while (reader.Read())
                    {
                        primaryKeyColumns.Add(reader[1].ToString());
                        if (string.IsNullOrEmpty(constraint_name)) constraint_name = reader[2].ToString();
                    }

                    var PrimaryKeyExist = primaryKeyColumns.Count > 0;

                    if (PrimaryKeyExist)
                    {
                        PrimaryKey primaryKey = new PrimaryKey
                        {
                            FullTableName = tableName,
                            ConstraintName = constraint_name,
                            ColumnNames = primaryKeyColumns
                        };
                        return primaryKey;

                    }
                    else
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get PRIMARY KEY for table: {tableName}", ex);
            }
        }
        protected override TableDataRows GetRangeOfRowsFromTable(FullTableName tableName, int startWith = 0, int ammountOfRows = 100)
        {
            var queryString =
                $"SELECT *\n" +
                $"FROM \"{tableName.SchemaCatalogName}\".\"{tableName.TableName}\"\n" +
                $"OFFSET {startWith}\n" +
                $"FETCH NEXT {ammountOfRows} ROWS ONLY;";

            TableDataRow[] dataRows = new TableDataRow[ammountOfRows];

            using (var reader = GetDataReader(queryString))
            {
                int ch = 0; // counter 
                while (reader.Read())
                {
                    dataRows[ch] = GetDataRowFromReader(reader);
                    ch++;
                }
            }

            return new TableDataRows(dataRows);
        }
        protected override List<SchemaSequence> GetSequences()
        {
            try
            {
                string queryString =
                        "select *\n" +
                        "from information_schema.sequences";
                List<SchemaSequence> sequences = new List<SchemaSequence>();

                using (var reader = GetDataReader(queryString))
                {
                    while (reader.Read())
                    {
                        sequences.Add(new SchemaSequence()
                        {
                            Sequence_catalog = reader[0].ToString(),
                            Sequence_schema = reader[1].ToString(),
                            Sequence_name = reader[2].ToString(),
                            Data_type = reader[3].ToString(),
                            Numeric_presicion = reader[4].ToString(),
                            Numeric_presicion_radix = reader[5].ToString(),
                            Numeric_scale = reader[6].ToString(),
                            Start_vlaue = reader[7].ToString(),
                            Minimum_value = reader[8].ToString(),
                            Maximum_value = reader[9].ToString(),
                            Increment = reader[10].ToString(),
                            Cycle_option = reader[11].ToString()
                        });
                    }
                    return sequences;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get SEQUENCES", ex);
            }
        }
        protected override List<SchemaColumn> GetTableColumns(FullTableName tableName)
        {
            try
            {
                string queryString =
                        "select *\n" +
                        "from information_schema.columns\n" +
                        $"where table_schema = '{tableName.SchemaCatalogName}'\n" +
                        $"and table_name = '{tableName.TableName}'\n" +
                        $"order by ordinal_position";
                List<SchemaColumn> columns = new List<SchemaColumn>();

                using (var reader = GetDataReader(queryString))
                {
                    while (reader.Read())
                    {
                        var column = GetSchemaColumn(reader);
                        columns.Add(column);
                    }
                    return columns;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get COLUMNS for table: {tableName}", ex);
            }
        }
        private SchemaColumn GetSchemaColumn(DbDataReader reader)
        {
            SchemaColumn column = new SchemaColumn
            {
                Table_catalog = reader[0].ToString(),
                Table_schema = reader[1].ToString(),
                Table_name = reader[2].ToString(),
                Column_name = reader[3].ToString(),
                Ordinal_position = reader[4].ToString(),
                Column_default = reader[5].ToString(),
                Is_nullable = reader[6].ToString() == "NO" ? "NOT NULL" : "NULL",
                Data_type = reader[7].ToString(),
                Character_maximum_length = reader[8].ToString(),
                Character_octet_length = reader[9].ToString(),
                Numeric_presicion = reader[10].ToString(),
                Numeric_presicion_radix = reader[11].ToString(),
                Numeric_scale = reader[12].ToString(),
                Datetime_presicion = reader[13].ToString(),
                Character_set_catalog = reader[15].ToString(),
                Character_set_schema = reader[16].ToString(),
                Character_set_name = reader[17].ToString(),
                Collation_catalog = reader[18].ToString(),
                Is_self_referencing = reader[33].ToString(),
                Is_identity = reader[34].ToString(),
                Identity_generation = reader[35].ToString(),
                Identity_start = reader[36].ToString(),
                Identity_increment = reader[37].ToString(),
                Identity_maximum = reader[38].ToString(),
                Identity_minimum = reader[39].ToString(),
                Identity_Cycle = reader[40].ToString(),
                Is_generated = reader[41].ToString(),
                Generation_expression = reader[42].ToString()
            };
            return column;
        }
        protected override List<FullTableName> GetTableNames()
        {
            DataTable schema = conn.GetSchema("Tables");
            var tableNames = new List<FullTableName>();
            foreach (DataRow row in schema.Rows)
            {
                var tableFullName = new FullTableName
                {
                    TableName = row[2].ToString(),
                    SchemaCatalogName = row[1].ToString()
                };
                tableNames.Add(tableFullName);
            }
            return tableNames;
        }
        protected override UniqueConstraint GetUnique(string uniqueName, FullTableName tableName)
        {
            try
            {
                var queryString =
                                "SELECT\n" +
                                "KC.COLUMN_NAME\n" +
                                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC\n" +
                                "JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KC\n" +
                                "ON KC.TABLE_NAME = TC.TABLE_NAME AND KC.TABLE_SCHEMA = TC.TABLE_SCHEMA\n" +
                                "AND KC.CONSTRAINT_NAME = TC.CONSTRAINT_NAME\n" +
                                $"WHERE TC.CONSTRAINT_TYPE = 'UNIQUE' AND TC.CONSTRAINT_NAME = '{uniqueName}'\n" +
                                $"AND TC.TABLE_NAME = '{tableName.TableName}'\n" +
                                $"AND TC.TABLE_SCHEMA = '{tableName.SchemaCatalogName}'";
                List<string> colunmNames = new List<string>();
                using (var reader = GetDataReader(queryString))
                {

                    while (reader.Read())
                    {
                        colunmNames.Add(reader[0].ToString());
                    }
                }

                var unique = new UniqueConstraint()
                {
                    FullTableName = tableName,
                    ColumnNames = colunmNames,
                    ConstraintName = uniqueName
                };

                return unique;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get Unique CONSTRAIN: {uniqueName}", ex);
            }
        }
        protected override List<string> GetUniqueConstraintNames(FullTableName tableName)
        {
            try
            {
                string queryString =
                    "SELECT\n" +
                    "   DISTINCT TC.CONSTRAINT_NAME\n" +
                    "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC\n" +
                    "JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KC\n" +
                    "   ON KC.TABLE_NAME = TC.TABLE_NAME AND KC.TABLE_SCHEMA = TC.TABLE_SCHEMA AND KC.CONSTRAINT_NAME = TC.CONSTRAINT_NAME\n" +
                    $"WHERE TC.CONSTRAINT_TYPE = 'UNIQUE' AND TC.TABLE_NAME = '{tableName}'\n" +
                    $"AND TC.TABLE_SCHEMA = '{tableName.SchemaCatalogName}'";


                List<string> uniques = new List<string>();
                using (var reader = GetDataReader(queryString))
                {

                    while (reader.Read())
                    {
                        uniques.Add(reader[0].ToString());
                    }
                }

                return uniques;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get UNIQUE CONSTRAINTS for table: {tableName}", ex);
            }
        }

        protected override List<string> GetSchemasNames()
        {
            try
            {
                string queryString =
                        "SELECT DISTINCT TABLE_SCHEMA\n" +
                        "FROM INFORMATION_SCHEMA.TABLES\n" +
                        "WHERE TABLE_SCHEMA <> 'pg_catalog'\n" +
                        "AND TABLE_SCHEMA <> 'information_schema'\n" +
                        "AND TABLE_SCHEMA <> 'public';";
                List<string> schemas = new List<string>();

                using (var reader = GetDataReader(queryString))
                {
                    while (reader.Read())
                    {
                        schemas.Add(reader[0].ToString());
                    }
                    return schemas;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get Schemas", ex);
            }
        }

        public override void CreateSchema(string schemaName)
        {
            var queryString = $"CREATE SCHEMA \"{schemaName}\"";
            ExecuteCommand(queryString);
        }

        protected override int ExecuteCommandScalar(string command)
        {
            try
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = command;
                var res = (int)(long)cmd.ExecuteScalar();
                return res;
            }
            catch (Exception e)
            {
                conn.Close();
                throw new Exception($"Invalid Operation:\n {command}", e);
            }
        }
    }
}