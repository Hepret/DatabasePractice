﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.ScriptCreators.DatabaseSchemaCreatingScriptsCreator
{
    class CreatorScriptsFromSchemaMssqlToMssql : ICreateInsertSchemaScripts
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
            var createSequenceStr =
                $"CREATE SEQUENCE {sequence.Sequence_name} " +
                $"INCREMENT BY {sequence.Increment} " +
                $"MINVALUE {sequence.Minimum_value} " +
                $"MAXVALUE {sequence.Maximum_value} " +
                $"START WITH {sequence.Start_vlaue};";

            return createSequenceStr;
        }

        #endregion

        #region CreatingTables
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
            StringBuilder createTableStr = new StringBuilder($"CREATE TABLE [{table.SchemaCatalog}].[{table.TableName}]\n(\n");
            
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
            createColumnStr.Append($"[{schemaColumn.Column_name}] {schemaColumn.Data_type}");
            if (schemaColumn.Is_generated == "1")
            {
                return CreateGeneratedStoredColumn(schemaColumn);
            }
            switch (schemaColumn.Data_type)
            {
                case "binary":
                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                    createColumnStr.Append($"({schemaColumn.Character_maximum_length})");
                    break;
                case "numeric":
                    if (string.IsNullOrEmpty(schemaColumn.Numeric_presicion)) break;
                    createColumnStr.Append($"({schemaColumn.Numeric_presicion},{schemaColumn.Numeric_scale})");
                    break;
                case "time":
                case "datetime":
                case "datetime2":
                case "datetimeoffset":
                    createColumnStr.Append($"({schemaColumn.Datetime_presicion})");
                    break;
            }
            createColumnStr.Append($" {schemaColumn.Is_nullable}");
            if (!string.IsNullOrEmpty(schemaColumn.Column_default)) createColumnStr.Append($" DEFAULT {schemaColumn.Column_default}");
            if (schemaColumn.Is_identity == "True" ) createColumnStr.Append(CreateIdentityForColumn(schemaColumn));
            return createColumnStr.ToString();
        }
        private string CreateGeneratedStoredColumn(SchemaColumn schemaColumn)
        {
            return $"[{schemaColumn.Column_name}] AS {schemaColumn.Generation_expression}";
        }
        private string CreateIdentityForColumn(SchemaColumn schemaColumn)
        {
            return $" IDENTITY " +
                $"({schemaColumn.Identity_start},{schemaColumn.Identity_increment})";
        }
        #endregion

        #region Creating Constraints
        private string CreateUnique(List<UniqueConstraint> uniques)
        {
            StringBuilder uniquesCreateString = new StringBuilder();

            
            foreach (var unique in uniques)
            {
                var tmpUniques = unique.ColumnNames.Select(name => $"[{name}]");
                string template = $",\nCONSTRAINT {unique.ConstraintName} UNIQUE({string.Join(",", tmpUniques)})";
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
                string template = $",\nCONSTRAINT {fk.ConstraintName} FOREIGN KEY ([{fk.ColumnName}]) REFERENCES [{fk.ReferencedSchema}].[{fk.ReferencedTable}] ([{fk.ReferencedColumn}])";
                fkString.Append(template);
            }
            return fkString.ToString();
        }
        private string CreatePrimaryKey(PrimaryKey primaryKey)
        {
            if (primaryKey == null) return "";
            var tmpColumnList = primaryKey.ColumnNames.Select(name => $"[{name}]");
            return $",\nCONSTRAINT {primaryKey.ConstraintName} PRIMARY KEY({string.Join(",",tmpColumnList)})";
        }
        #endregion
        #endregion

        
    }
}
