using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator
{
    public class CreatorScriptsFromSchemaPostgresqlToPostgresql : ICreateInsertSchemaScripts
    {
        public DatabaseSchemaCreatingScript CreateScriptsForInsertSchema(SchemaDatabase schemaDatabase, string databaseNewName)
        {
            var script = new DatabaseSchemaCreatingScript(schemaDatabase, databaseNewName)
            {
                CreateDatabaseScript = new CreateDatabaseScript(databaseNewName)
                {
                    Script = CreateDatabase(databaseNewName)
                },
                CreateExtensionScript = CreateExtensions(schemaDatabase.Extensions),
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
                CreateTablesScripts = CreateTables(schemaDatabase.Tables),
                CreateExtensionScript = CreateExtensions(schemaDatabase.Extensions)
            };
            return script;
        }

        private string[] CreateExtensions(List<DatabaseExtension> extensions)
        {
            var scripts = new string[extensions.Count];
            for (int i = 0; i < extensions.Count; i++)
            {
                scripts[i] = CreateExtension(extensions[i]);
            }

            return scripts;
        }

        private string CreateExtension(DatabaseExtension extension)
        {
            return $"CREATE EXTENSION IF NOT EXISTS {extension.Name};";
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
                $"CREATE SEQUENCE \"{sequence.SequenceSchema}\".\"{sequence.SequenceName}\" " +
                $"INCREMENT BY {sequence.Increment} " +
                $"MINVALUE {sequence.MinimumValue} " +
                $"MAXVALUE {sequence.MaximumValue} " +
                $"START WITH {sequence.LastValue};";

            return createSequenceStr;
        }
        
        #endregion

        #region Creating Tables
        private CreateTablesScripts CreateTables(List<SchemaTable> tables)
        {
            var createTablesScripts = new CreateTablesScripts(tables);

            for (var i = 0; i < createTablesScripts.Count; i++)
            {
                var script = CreateTable(tables[i]);
                createTablesScripts[i].Script = script;
            }

            return createTablesScripts;
        }
        
        private string CreateTable(SchemaTable table)
        {
            StringBuilder createTableStr = new StringBuilder($"CREATE TABLE IF NOT EXISTS \"{table.SchemaCatalog}\".\"{table.TableName}\"\n(" + $"\n");

            string columns = CreateColumns(table.Columns);
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

        private string CreateColumns(List<SchemaColumn> schemaColumns )
        {
            string[] columnArr = new string[schemaColumns.Count];
            for (int i = 0; i < schemaColumns.Count; i++)
            {
                var column = CreateColumn(schemaColumns[i]);
                columnArr[i] = column;
            }
            string columns = string.Join(",\n", columnArr);
            return columns;
        }

        private string CreateColumn(SchemaColumn schemaColumn)
        {
             StringBuilder createColumnStr = new StringBuilder();

             if (schemaColumn.DataType == "ARRAY")
             {
                 createColumnStr.Append($"\"{schemaColumn.ColumnName}\" {schemaColumn.UdtName}");
             }
             else if (schemaColumn.DataType == "USER-DEFINED")
             {
                 createColumnStr.Append($"\"{schemaColumn.ColumnName}\" \"public\".{schemaColumn.UdtName}");
             }
             else
             {
                 createColumnStr.Append($"\"{schemaColumn.ColumnName}\" {schemaColumn.DataType}");
             }
             switch (schemaColumn.DataType)
             {
                 case "bit":
                 case "varbit":
                 case "character":
                 case "character varying":
                     createColumnStr.Append($"({schemaColumn.CharacterMaximumLength})");
                     break;
                 case "numeric":
                     if (string.IsNullOrEmpty(schemaColumn.NumericPresicion)) break;
                     createColumnStr.Append($"({schemaColumn.NumericPresicion},{schemaColumn.NumericScale})");
                     break;
                 case "time":
                 case "timestamp":
                     createColumnStr.Append($"({schemaColumn.DatetimePresicion})");
                     break;

             }
             createColumnStr.Append($" {schemaColumn.IsNullable}");
             if (!string.IsNullOrEmpty(schemaColumn.ColumnDefault)) createColumnStr.Append($" DEFAULT {schemaColumn.ColumnDefault}");
             if (schemaColumn.IsIdentity == "YES") createColumnStr.Append(CreateIdentityForColumn(schemaColumn));
             if (schemaColumn.IsGenerated == "ALWAYS") createColumnStr.Append(CreateGeneratedStoredColumn(schemaColumn));
             return createColumnStr.ToString();
        }
        private string CreateGeneratedStoredColumn(SchemaColumn schemaColumn)
        {
            return $" GENERATED ALWAYS AS {schemaColumn.GenerationExpression} STORED";
        }
        private string CreateIdentityForColumn(SchemaColumn schemaColumn)
        {
            var identityStringBld = new StringBuilder();
            identityStringBld.Append(
                $" GENERATED {schemaColumn.IdentityGeneration} AS IDENTITY " +
                $"(INCREMENT BY {schemaColumn.IdentityIncrement} " +
                $"MINVALUE {schemaColumn.IdentityMinimum} " +
                $"MAXVALUE {schemaColumn.IdentityMaximum} "
            );

            if (schemaColumn.IdentityCycle == "YES") identityStringBld.Append("CYCLE");

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
