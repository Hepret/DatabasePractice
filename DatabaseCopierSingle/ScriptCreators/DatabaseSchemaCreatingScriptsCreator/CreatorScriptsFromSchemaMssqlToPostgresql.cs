using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator
{
    public class CreatorScriptsFromSchemaMssqlToPostgresql : ICreateInsertSchemaScripts
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
        #region Creating Sequences
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
            var schema = sequence.SequenceSchema == "dbo" ? "public" : sequence.SequenceSchema;
            var createSequenceStr =
                $"CREATE SEQUENCE \"{schema}\".\"{sequence.SequenceName}\" " +
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

            var schema = table.SchemaCatalog == "dbo" ? "public" : table.SchemaCatalog;
            StringBuilder createTableStr = new StringBuilder($"CREATE TABLE \"{schema}\".\"{table.TableName}\"\n(" + $"\n");

            string columns = CreateColumns(table.Columns);
            string pk = CreatePrimaryKey(table.PrimaryKey);
            string fk = CreateForeignKey(table.ForeignKeys);
            string unique = CreateUnique(table.UniqueConstraints);
            string checks = CreateCheckConstraint(table.CheckConstraints);

            if (!string.IsNullOrEmpty(columns)) createTableStr.Append(columns);
            if (!string.IsNullOrEmpty(pk)) createTableStr.Append(pk);
            if (!string.IsNullOrEmpty(fk)) createTableStr.Append(fk);
            if (!string.IsNullOrEmpty(unique)) createTableStr.Append(unique);
            //TODO add ability to add check constraints 
            //if (!string.IsNullOrEmpty(checks)) createTableStr.Append(checks);

            createTableStr.AppendLine("\n);");
            return createTableStr.ToString();
        }
        
        private string CreateColumns(List<SchemaColumn> schemaColumns)
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
            var dataType = TypesFromMssqlToPostgresql.Get(schemaColumn.DataType);
            if (schemaColumn.IsGenerated == "1")
            {
                return CreateGeneratedStoredColumn(schemaColumn);
            }
            createColumnStr.Append($"\"{schemaColumn.ColumnName}\" {dataType}");
            switch (dataType)
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
            if (!string.IsNullOrEmpty(schemaColumn.ColumnDefault)) createColumnStr.Append($" DEFAULT {CreateDefault(schemaColumn.ColumnDefault)}");
            if (schemaColumn.IsIdentity == "True" ) createColumnStr.Append(CreateIdentityForColumn(schemaColumn));
            return createColumnStr.ToString();
        }

        private static string CreateDefault(string schemaColumnColumnDefault)
        {
            if (schemaColumnColumnDefault == "GETDATE()") return "now()";

            const string pattern = @"\(NEXT\sVALUE\sFOR\s\[ (\w*) \]  \.? \[? (\w*)? \]? \)";
            foreach (Match match in Regex.Matches(schemaColumnColumnDefault, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                var sequenceName = string.IsNullOrEmpty(match.Groups[2].Value)
                    ? match.Groups[1].Value
                    : match.Groups[2].Value;

                return $"nextval('{sequenceName}'::regclass)";
            }
            return schemaColumnColumnDefault;
            
        }
        
        private string CreateGeneratedStoredColumn(SchemaColumn schemaColumn)
        {
            return $" GENERATED ALWAYS AS {schemaColumn.GenerationExpression} STORED";
        }
        
        
        private string CreateIdentityForColumn(SchemaColumn schemaColumn)
        {
            var identityStringBld = new StringBuilder();
            identityStringBld.Append(
                $" GENERATED ALWAYS AS IDENTITY " +
                $"(INCREMENT BY {schemaColumn.IdentityIncrement} " +
                $"START WITH  {schemaColumn.IdentityStart})"
            );
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
                string referencedSchema = fk.ReferencedSchema == "dbo" ? "public" : fk.ReferencedSchema;
                string template = $",\nCONSTRAINT {fk.ConstraintName} FOREIGN KEY (\"{fk.ColumnName}\") REFERENCES \"{referencedSchema}\".\"{fk.ReferencedTable}\" (\"{fk.ReferencedColumn}\")";
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
    }
}