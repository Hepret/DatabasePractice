using DatabaseCopierSingle.DatabaseTableComponents;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniqueConstraint = DatabaseCopierSingle.DatabaseTableComponents.UniqueConstraint;

namespace DatabaseCopierSingle.DatabaseProviders
{
    class MSSQLProvider : DatabaseProvider
    {
        public MSSQLProvider(string connectionString) : base(new SqlConnection(connectionString)) { }
        protected override List<CheckConstraint> GetCheckConstraints(FullTableName tableName)
        {
            List<CheckConstraint> checkConstraints = new List<CheckConstraint>();
            var queryString = "SELECT TABLE_NAME, TC.CONSTRAINT_NAME, CHECK_CLAUSE\n" +
                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS AS TC\n" +
                "INNER JOIN INFORMATION_SCHEMA.CHECK_CONSTRAINTS AS CC\n" +
                "ON TC.CONSTRAINT_NAME = CC.CONSTRAINT_NAME\n" +
                $"WHERE TABLE_NAME = '{tableName.TableName}'\n" +
                $"AND TABLE_SCHEMA = '{tableName.SchemaCatalogName}' "; 
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
                            });
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
                    "SELECT  obj.name AS FK_NAME,\n" +
                    "schem1.name AS [ShemaCatalog],\n" +
                    "tab1.name AS[table],\n" +
                    "col1.name AS[column],\n" +
                    "schem2.name AS [referenced_schema],\n" +
                    "tab2.name AS[referenced_table],\n" +
                    "col2.name AS[referenced_column]\n" +
                    "FROM sys.foreign_key_columns fkc\n" +
                    "INNER JOIN sys.objects obj\n" +
                    "ON obj.object_id = fkc.constraint_object_id\n" +
                    "INNER JOIN sys.tables tab1\n" +
                    "ON tab1.object_id = fkc.parent_object_id\n" +
                    "INNER JOIN sys.schemas sch\n" +
                    "ON tab1.schema_id = sch.schema_id\n" +
                    "INNER JOIN sys.columns col1\n" +
                    "    ON col1.column_id = parent_column_id AND col1.object_id = tab1.object_id\n" +
                    "INNER JOIN sys.tables tab2\n" +
                    "    ON tab2.object_id = fkc.referenced_object_id\n" +
                    "INNER JOIN sys.columns col2\n" +
                    "    ON col2.column_id = referenced_column_id AND col2.object_id = tab2.object_id\n" +
                    "INNER JOIN sys.schemas schem1\n" +
                    "    ON schem1.schema_id = tab1.schema_id\n" +
                    "INNER JOIN sys.schemas schem2\n" +
                    "    ON schem2.schema_id = tab2.schema_id\n" +
                    $"WHERE tab1.name = '{tableName.TableName}'" +
                    $"AND schem1.name = '{tableName.SchemaCatalogName}'";

                using (var reader = GetDataReader(queryString))
                {

                    while (reader.Read())
                    {
                        var constraintName = reader[0].ToString();
                        var schemaCatalog = reader[1].ToString();
                        var tablName = reader[2].ToString();
                        var columnName = reader[3].ToString();
                        var referecedSchema = reader[4].ToString();
                        var referencedTable = reader[5].ToString();
                        var referencedColumn = reader[6].ToString();
                        


                        foreignKeys.Add(
                            new ForeignKey
                        
                            {
                                ConstraintName = constraintName,
                                TableName = tablName,
                                ColumnName = columnName,
                                SchemaCatalog = schemaCatalog,
                                ReferencedSchema = referecedSchema,
                                ReferencedTable = referencedTable,
                                ReferencedColumn = referencedColumn
                            });
                    }
                    return foreignKeys;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get foreign keys for table: {tableName.TableName}", ex);
            }
        }

        protected override int GetNumberOfRowsInTheTable(FullTableName tableName)
        {
            var quaryString =
                    $"SELECT COUNT(*)\n" +
                    $"FROM [{tableName.SchemaCatalogName}].[{tableName.TableName}]";
            var RowsAmmount = ExecuteCommandScalar(quaryString);
            return RowsAmmount;
        }

        protected override PrimaryKey GetPrimaryKey(FullTableName tableName)
        {
            try
            {
                string queryString =
                        "SELECT\n" +
                        "    TC.TABLE_NAME,\n" +
                        "    KC.COLUMN_NAME,\n" +
                        "    TC.CONSTRAINT_NAME\n" +
                        "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS TC\n" +
                        "JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE KC\n" +
                        "    ON KC.TABLE_NAME = TC.TABLE_NAME AND KC.table_schema = TC.table_schema AND KC.CONSTRAINT_NAME = TC.CONSTRAINT_NAME\n" +
                        $"WHERE TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND TC.TABLE_NAME = '{tableName.TableName}'\n" +
                        "            AND KC.ordinal_position IS NOT NULL\n" +
                        $"           AND TC.TABLE_SCHEMA = '{tableName.SchemaCatalogName}'\n" +
                        "ORDER BY TC.TABLE_SCHEMA,\n" +
                        "         TC.TABLE_NAME,\n" +
                        "         KC.ORDINAL_POSITION";

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
                throw new Exception($"Can't get PRIMARY KEY for table: {tableName.TableName}", ex);
            }
        }

        protected override TableDataRows GetRangeOfRowsFromTable(FullTableName tableName, int startWith = 0, int ammountOfRows = 100)
        {
            var queryString =
                $"SELECT *\n" +
                $"FROM [{tableName.SchemaCatalogName}].[{tableName.TableName}]\n" +
                $"ORDER BY(SELECT NULL)\n" +
                $"OFFSET {startWith} ROWS\n" +
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
                        "select isc.*, ic.is_identity, ic.seed_value, ic.increment_value, cc.is_computed ,cc.[definition]\n" +
                        "from sys.all_columns ac\n" +
                        "left join sys.identity_columns ic\n" +
                        "on ac.object_id = ic.object_id\n" +
                        "and ac.column_id = ic.column_id\n" +
                        "join sys.tables st\n" +
                        "on ac.object_id = st.object_id\n" +
                        "left join sys.computed_columns as cc\n" +
                        "on cc.column_id = ac.column_id\n" +
                        "and cc.object_id = st.object_id\n" +
                        "join INFORMATION_SCHEMA.COLUMNS isc\n" +
                        "on st.name = isc.TABLE_NAME and ac.name = isc.COLUMN_NAME\n" +
                        $"where isc.TABLE_NAME = '{tableName.TableName}'\n" +
                        $"and isc.TABLE_SCHEMA = '{tableName.SchemaCatalogName}'";


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
                Is_identity = reader[23].ToString(),
                Identity_start = reader[24].ToString(),
                Identity_increment = reader[25].ToString(),
                Is_generated = reader[26].ToString(),
                Generation_expression = reader[27].ToString()
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
                throw new Exception($"Can't get Unique CONSTRAINT: {uniqueName}", ex);
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
                    $"WHERE TC.CONSTRAINT_TYPE = 'UNIQUE' AND TC.TABLE_NAME = '{tableName.TableName}'\n" +
                    $"AND TC.TABLE_SCHEMA = '{tableName.SchemaCatalogName}'";

                List<string> uniques = new List<string>();
                using (var reader = GetDataReader(queryString))
                {
                    while (reader.Read())
                    {
                        var constraintName = reader[0].ToString();
                        uniques.Add(constraintName);
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
                        "FROM INFORMATION_SCHEMA.TABLES;";
                List<string> schemas = new List<string>();

                using (var reader = GetDataReader(queryString))
                {
                    while (reader.Read())
                    {
                        var schema = reader[0].ToString();
                        if (schema == "dbo") continue;
                        schemas.Add(schema);

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
            try
            {
                var queryString = $"CREATE SCHEMA [{schemaName}]";
                ExecuteCommand(queryString);
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't create SCHEMA: {schemaName}", ex);
            }
        }

        protected override int ExecuteCommandScalar(string command)
        {
            try
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = command;
                var res = (int)cmd.ExecuteScalar();
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
