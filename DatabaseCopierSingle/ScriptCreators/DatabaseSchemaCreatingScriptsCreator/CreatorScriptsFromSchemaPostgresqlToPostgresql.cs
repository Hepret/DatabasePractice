using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator
{
    class CreatorScriptsFromSchemaPostgresqlToPostgresql : ICreateInsertSchemaScripts
    {
        public DatabaseSchemaCreatingScript CreateScriptsForInsertSchema(SchemaDatabase schemaDatabase, string databaseNewName)
        {
            var script = new DatabaseSchemaCreatingScript(schemaDatabase, databaseNewName)
            {
                CreateDatabaseScript = new CreateDatabaseScript(databaseNewName)
                {
                    Script = CreateDatabase(databaseNewName)
                },
                CreateSchemasScripts = CreateSchemas(schemaDatabase.Schemas),
                CreateSequencesScripts = CreateSequences(schemaDatabase.Sequences),
                CreateTablesScripts = CreateTables(schemaDatabase.Tables)
            };
            return script;
        }
        
        public DatabaseSchemaCreatingScript CreateScriptsForInsertSchema(SchemaDatabase schemaDatabase)
        {
            var script = new DatabaseSchemaCreatingScript(schemaDatabase)
            {
                CreateSchemasScripts = CreateSchemas(schemaDatabase.Schemas),
                CreateSequencesScripts = CreateSequences(schemaDatabase.Sequences),
                CreateTablesScripts = CreateTables(schemaDatabase.Tables)
            };
            return script;
        }

        #region Creating Database
        private string CreateDatabase(string databaseName)
        {
            return $"CREATE DATABASE {databaseName}";
        }
        #endregion

        #region Creating Schemas
        private string[] CreateSchemas(List<string> schemaDatabaseSchemas)
        {
            var createSchemasScripts = new string[schemaDatabaseSchemas.Count];
            for (int i = 0; i < schemaDatabaseSchemas.Count; i++)
            {
                var template = $"CREATE SCHEMA {schemaDatabaseSchemas[i]}";
                createSchemasScripts[i] = template;
            }

            return createSchemasScripts;
        }
        #endregion

        #region Creating Sequeneces
        
        private string[] CreateSequences(List<SchemaSequence> sequences)
        {
            string[] createSequenceArr = new string[sequences.Count]; // Array of create seq commands
            for (int i = 0; i < sequences.Count; i++)
            {
                createSequenceArr[i] = CreateSequence(sequences[i]);
            }

            return createSequenceArr;
        }
        private string CreateSequence(SchemaSequence sequence)
        {
            var createSequenceStr =
                $"CREATE SEQUENCE \"{sequence.Sequence_schema}\".\"{sequence.Sequence_name}\" " +
                $"INCREMENT BY {sequence.Increment} " +
                $"MINVALUE {sequence.Minimum_value} " +
                $"MAXVALUE {sequence.Maximum_value} " +
                $"START WITH {sequence.Start_vlaue};";

            return createSequenceStr;
        }
        

        #endregion

        #region Creating Tables
        private CreateTablesScripts CreateTables(List<SchemaTable> tables)
        {
            var createTablesScripts = new CreateTablesScripts(tables);

            for (var i = 0; i < createTablesScripts.Count; i++)
            {
                var script = tables[i].HasSelfReference 
                    ? CreateTableWithSelfReference(tables[i]) :
                    CreateTable(tables[i]);
                createTablesScripts[i].Script = script;
            }

            return createTablesScripts;
        }

        private string CreateTableWithSelfReference(SchemaTable table)
        {
            StringBuilder createTableStr = new StringBuilder($"CREATE TABLE \"{table.SchemaCatalog}\".\"{table.TableName}\"\n(" + $"\n");
            string columns = CreateColumns(table.Columns, table.HasSelfReference);
            string pk = CreatePrimaryKey(table.PrimaryKey);
            string fk = CreateForeignKey(table.ForeignKeys.Where(fork => fork.IsSelfReference != true));
            string unique = CreateUnique(table.UniqueConstraints);
            string checks = CreateCheckConstraint(table.CheckConstraints);
            
            if (!string.IsNullOrEmpty(columns)) createTableStr.Append(columns);
            if (!string.IsNullOrEmpty(pk)) createTableStr.Append(pk);
            if (!string.IsNullOrEmpty(fk)) createTableStr.Append(fk);
            if (!string.IsNullOrEmpty(unique)) createTableStr.Append(unique);
            if (!string.IsNullOrEmpty(checks)) createTableStr.Append(checks);
            
            createTableStr.AppendLine("\n);");
            return createTableStr.ToString();
        }

        private string CreateTable(SchemaTable table)
        {
            StringBuilder createTableStr = new StringBuilder($"CREATE TABLE \"{table.SchemaCatalog}\".\"{table.TableName}\"\n(" + $"\n");

            string columns = CreateColumns(table.Columns, table.HasSelfReference);
            string pk = CreatePrimaryKey(table.PrimaryKey);
            string fk = CreateForeignKey(table.ForeignKeys);
            string unique = CreateUnique(table.UniqueConstraints);
            string checks = CreateCheckConstraint(table.CheckConstraints);

            if (!string.IsNullOrEmpty(columns)) createTableStr.Append(columns);
            if (!string.IsNullOrEmpty(pk)) createTableStr.Append(pk);
            if (!string.IsNullOrEmpty(fk)) createTableStr.Append(fk);
            if (!string.IsNullOrEmpty(unique)) createTableStr.Append(unique);
            if (!string.IsNullOrEmpty(checks)) createTableStr.Append(checks);

            createTableStr.AppendLine("\n);");
            return createTableStr.ToString();
        }

        #region Creating Columns 

        private string CreateColumns(List<SchemaColumn> schemaColumns, bool tableHasSelfReference )
                {
                    string[] columnArr = new string[schemaColumns.Count];
                    for (int i = 0; i < schemaColumns.Count; i++)
                    {
                        var column = CreateColumn(schemaColumns[i], tableHasSelfReference);
                        columnArr[i] = column;
                    }
                    string columns = string.Join(",\n", columnArr);
                    return columns;
                }

        private string CreateColumn(SchemaColumn schemaColumn, bool tableHasSelfReference = false)
                 {
                     StringBuilder createColumnStr = new StringBuilder();
                     createColumnStr.Append($"\"{schemaColumn.Column_name}\" {schemaColumn.Data_type}");
                     switch (schemaColumn.Data_type)
                     {
                         case "bit":
                         case "varbit":
                         case "character":
                         case "character varying":
                             createColumnStr.Append($"({schemaColumn.Character_maximum_length})");
                             break;
                         case "numeric":
                             if (string.IsNullOrEmpty(schemaColumn.Numeric_presicion)) break;
                             createColumnStr.Append($"({schemaColumn.Numeric_presicion},{schemaColumn.Numeric_scale})");
                             break;
                         case "time":
                         case "timestamp":
                             createColumnStr.Append($"({schemaColumn.Datetime_presicion})");
                             break;
                     }
                     createColumnStr.Append($" {schemaColumn.Is_nullable}");
                     if (!string.IsNullOrEmpty(schemaColumn.Column_default)) createColumnStr.Append($" DEFAULT {schemaColumn.Column_default}");
                     if (schemaColumn.Is_identity == "YES" && !tableHasSelfReference) createColumnStr.Append(CreateIdentityForColumn(schemaColumn));
                     if (schemaColumn.Is_generated == "ALWAYS") createColumnStr.Append(CreateGeneratedStoredColumn(schemaColumn));
                     return createColumnStr.ToString();
                 }
        private string CreateGeneratedStoredColumn(SchemaColumn schemaColumn)
        {
            return $" GENERATED ALWAYS AS {schemaColumn.Generation_expression} STORED";
        }
        private string CreateIdentityForColumn(SchemaColumn schemaColumn)
        {
            var identityStringBld = new StringBuilder();
            identityStringBld.Append(
                $" GENERATED {schemaColumn.Identity_generation} AS IDENTITY " +
                $"(INCREMENT BY {schemaColumn.Identity_increment} " +
                $"MINVALUE {schemaColumn.Identity_minimum} " +
                $"MAXVALUE {schemaColumn.Identity_maximum} "
            );

            if (schemaColumn.Identity_Cycle == "YES") identityStringBld.Append("CYCLE");

            identityStringBld.Append(") ");

            return identityStringBld.ToString();

        }

        #endregion

        #region Creating Constraints

        private string CreateUnique(List<UniqueConstraint> uniques)
        {
            StringBuilder uniquesCreateString = new StringBuilder();

            foreach (var unique in uniques)
            {
                var tmpUniqueNames = unique.ColumnNames.Select(name => $"\"{name}\"");
                string template = $",\nCONSTRAINT {unique.ConstraintName} UNIQUE({string.Join(",", tmpUniqueNames)}";
                uniquesCreateString.Append(template);
            }
            return uniquesCreateString.ToString();
        }
        private string CreateCheckConstraint(List<CheckConstraint> checkConstraints)
        {
            StringBuilder checkConstraintString = new StringBuilder();

            foreach (var checkConstraint in checkConstraints)
            {
                string template = $",\nCONSTRAINT {checkConstraint.ConstraintName} " +
                                  $"CHECK ({checkConstraint.CheckClause})";
                checkConstraintString.Append(template);
            }

            return checkConstraintString.ToString();

        }
        private string CreateForeignKey(IEnumerable<ForeignKey> foreignKeys)
        {
            StringBuilder fkString = new StringBuilder();

            foreach (ForeignKey fk in foreignKeys)
            {
                string template = $",\nCONSTRAINT {fk.ConstraintName} FOREIGN KEY (\"{fk.ColumnName}\") REFERENCES \"{fk.ReferencedSchema}\".\"{fk.ReferencedTable}\" (\"{fk.ReferencedColumn}\")";
                fkString.Append(template);
            }
            return fkString.ToString();
        }
        private string CreatePrimaryKey(PrimaryKey primaryKey)
        {
            if (primaryKey == null) return "";
            var tmpPrimaryKeyColumns = primaryKey.ColumnNames.Select(name => $"\"{name}\"");
            return $",\nCONSTRAINT {primaryKey.ConstraintName} PRIMARY KEY({string.Join(",", tmpPrimaryKeyColumns)})";
        }

        #endregion
        #endregion

    }
}
