using System;
using System.Collections.Generic;
using System.Data.Common;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;
using UniqueConstraint = DatabaseCopierSingle.DatabaseTableComponents.UniqueConstraint;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers
{
    public class PostgresqlSchemaReceiver : DatabaseSchemaReceiver
    {
        public PostgresqlSchemaReceiver(DatabaseProvider provider) : base(provider)
        {
            
        }

        protected override List<string> GetSchemasNames()
        {
            try
            {
                string queryString =
                    "SELECT DISTINCT \"table_schema\"\n" +
                    "FROM \"information_schema\".\"tables\"\n" +
                    "WHERE \"table_schema\" != 'pg_catalog'\n" +
                    "AND \"table_schema\" != 'information_schema'\n" +
                    "AND \"table_schema\" != 'public';";
                List<string> schemas = new List<string>();

                using (var reader = Provider.GetDataReader(queryString))
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

        protected override List<DatabaseExtension> GetDatabaseExtensions()
        {
            try
            {
                string queryString =
                    "SELECT extname\n" +
                    "FROM pg_extension;";
                var extensions = new List<DatabaseExtension>();

                using (var reader = Provider.GetDataReader(queryString))
                {
                    while (reader.Read())
                    {
                        extensions.Add(new DatabaseExtension()
                        {
                            Name = reader[0].ToString()
                        });
                    }
                }
                
                return extensions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get extensions", ex);
            }   
        }

        protected override List<SchemaSequence> GetSequences()
        {
            try
            {
                string queryString =
                    "select *\n" +
                    "from information_schema.sequences";
                List<SchemaSequence> sequences = new List<SchemaSequence>();

                using (var reader = Provider.GetDataReader(queryString))
                {
                    while (reader.Read())
                    {
                        sequences.Add(new SchemaSequence()
                        {
                            SequenceCatalog = reader[0].ToString(),
                            SequenceSchema = reader[1].ToString(),
                            SequenceName = reader[2].ToString(),
                            DataType = reader[3].ToString(),
                            NumericPresicion = reader[4].ToString(),
                            NumericPresicionRadix = reader[5].ToString(),
                            NumericScale = reader[6].ToString(),
                            StartValue = reader[7].ToString(),
                            MinimumValue = reader[8].ToString(),
                            MaximumValue = reader[9].ToString(),
                            Increment = reader[10].ToString(),
                            CycleOption = reader[11].ToString()
                        });
                    }
                }
                
                GetSequencesLastValue(sequences);
                return sequences;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get SEQUENCES", ex);
            }
        }

        private void GetSequencesLastValue(List<SchemaSequence> sequences)
        {
            foreach (var sequence in sequences)
            {
                var queryString =
                    "SELECT LAST_VALUE\n" +
                    $"FROM \"{sequence.SequenceSchema}\".\"{sequence.SequenceName}\";";

                
                using (var reader  = Provider.GetDataReader(queryString))
                {
                    reader.Read();
                    sequence.LastValue = reader[0];
                }
            }
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
                List<string> columnNames = new List<string>();
                using (var reader = Provider.GetDataReader(queryString))
                {

                    while (reader.Read())
                    {
                        columnNames.Add(reader[0].ToString());
                    }
                }

                var unique = new UniqueConstraint()
                {
                    FullTableName = tableName,
                    ColumnNames = columnNames,
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
                using (var reader = Provider.GetDataReader(queryString))
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
                    "   ccu.table_schema,\n" +                       
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

                using (var reader =Provider.GetDataReader(queryString))
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

                using (var reader = Provider.GetDataReader(queryString))
                {
                    List<string> primaryKeyColumns = new List<string>();
                    string constraintName = "";
                    while (reader.Read())
                    {
                        primaryKeyColumns.Add(reader[1].ToString());
                        if (string.IsNullOrEmpty(constraintName)) constraintName = reader[2].ToString();
                    }

                    var primaryKeyExist = primaryKeyColumns.Count > 0;

                    if (primaryKeyExist)
                    {
                        PrimaryKey primaryKey = new PrimaryKey
                        {
                            FullTableName = tableName,
                            ConstraintName = constraintName,
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

               using (var reader = Provider.GetDataReader(queryString))
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
       
       protected  override SchemaColumn GetSchemaColumn(DbDataReader reader)
       {
           SchemaColumn column = new SchemaColumn
           {
               TableCatalog = reader[0].ToString(),
               TableSchema = reader[1].ToString(),
               TableName = reader[2].ToString(),
               ColumnName = reader[3].ToString(),
               OrdinalPosition = reader[4].ToString(),
               ColumnDefault = reader[5].ToString(),
               IsNullable = reader[6].ToString() == "NO" ? "NOT NULL" : "NULL",
               DataType = reader[7].ToString(),
               CharacterMaximumLength = reader[8].ToString(),
               CharacterOctetLength = reader[9].ToString(),
               NumericPresicion = reader[10].ToString(),
               NumericPresicionRadix = reader[11].ToString(),
               NumericScale = reader[12].ToString(),
               DatetimePresicion = reader[13].ToString(),
               CharacterSetCatalog = reader[15].ToString(),
               CharacterSetSchema = reader[16].ToString(),
               CharacterSetName = reader[17].ToString(),
               CollationCatalog = reader[18].ToString(),
               IsSelfReferencing = reader[33].ToString(),
               IsIdentity = reader[34].ToString(),
               IdentityGeneration = reader[35].ToString(),
               IdentityStart = reader[36].ToString(),
               IdentityIncrement = reader[37].ToString(),
               IdentityMaximum = reader[38].ToString(),
               IdentityMinimum = reader[39].ToString(),
               IdentityCycle = reader[40].ToString(),
               IsGenerated = reader[41].ToString(),
               GenerationExpression = reader[42].ToString(),
               UdtName = reader[27].ToString()
           };
           return column;
       }

       protected override List<FullTableName> GetTableNames()
       {

           var queryString =
               "SELECT \"table_schema\", \"table_name\"\n" +
               "FROM \"information_schema\".\"tables\"\n" +
               "WHERE \"table_schema\" != 'pg_catalog'\n" +
               "    AND \"table_schema\" != 'information_schema'" +
               "    AND \"table_type\" = 'BASE TABLE'";
           var tableNames = new List<FullTableName>();
           using (var reader = Provider.GetDataReader(queryString))
           {

               while (reader.Read())
               {
                   var schemaCatalog = reader[0].ToString();
                   var tableName = reader[1].ToString();
                   
                   tableNames.Add(new FullTableName()
                   {
                       SchemaCatalogName = schemaCatalog,
                       TableName = tableName
                   });
               }
           }
           return tableNames;
       }

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
               using (var reader = Provider.GetDataReader(queryString))
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

    }
}