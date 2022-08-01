﻿using DatabaseCopierSingle.DatabaseTableComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.ScriptCreators
{
    class CreatorScriptsFromSchemaPostgresqlToMSSQL
    {
        public static string CreateDatabase(SchemaDatabase schema)
        {
            return $"CREATE DATABASE {schema.DatabaseName}";
        }

        public static string CreateTableConstraintsSequences(SchemaDatabase schema)
        {
            StringBuilder CreateCmdStr = new StringBuilder();
            CreateCmdStr.AppendLine(CreateSequences(schema.Sequences)); // add seq
            CreateCmdStr.AppendLine(CreateTables(schema.Tables)); // tables include constraints
            return CreateCmdStr.ToString();
        }

        private static string CreateTables(List<SchemaTable> tables)
        {
            string[] createTablesArr = new string[tables.Count]; // set of create seq commands
            for (int i = 0; i < tables.Count; i++)
            {
                createTablesArr[i] = CreateTable(tables[i]);
            }
            string createTablesStr = string.Join("\n", createTablesArr);
            return createTablesStr;
        }
        private static string CreateTable(SchemaTable table)
        {
            StringBuilder createTableStr = new StringBuilder($"CREATE TABLE {table.TableName}\n(\n");

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

        private static string CreateColumns(List<SchemaColumn> schemaColumns)
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
        private static string CreateSequences(List<SchemaSequence> sequences)
        {
            string[] createSequanceArr = new string[sequences.Count]; // set of create seq commands
            for (int i = 0; i < sequences.Count; i++)
            {
                createSequanceArr[i] = CreateSequence(sequences[i]);
            }

            string createSequencesStr = string.Join("\n", createSequanceArr);

            return createSequencesStr;
        }
        private static string CreateSequence(SchemaSequence sequence)
        {
            var createSequenceStr =
                $"CREATE SEQUENCE {sequence.Sequence_name} " +
                $"INCREMENT BY {sequence.Increment} " +
                $"MINVALUE {sequence.Minimum_value} " +
                $"MAXVALUE {sequence.Maximum_value} " +
                $"START WITH {sequence.Start_vlaue};";

            return createSequenceStr;
        }
        private static string CreateUnique(List<UniqueConstraint> uniques)
        {
            StringBuilder uniquesCreateString = new StringBuilder();

            foreach (var unique in uniques)
            {
                string template = $",\nCONSTRAINT {unique.ConstraintName} UNIQUE({string.Join(",", unique.ColumnNames)}";
                uniquesCreateString.Append(template);
            }
            return uniquesCreateString.ToString();
        }
        private static string CreateCheckConstraint(List<CheckConstraint> checkConstraints)
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
        private static string CreateForeignKey(List<ForeignKey> foreignKeys)
        {
            StringBuilder fkString = new StringBuilder();

            foreach (ForeignKey fk in foreignKeys)
            {
                string template = $",\nCONSTRAINT {fk.ConstraintName} FOREIGN KEY ({fk.ColumnName}) REFERENCES {fk.ReferencedTable} ({fk.ReferencedColumn})";
                fkString.Append(template);
            }
            return fkString.ToString();
        }
        private static string CreatePrimaryKey(PrimaryKey primaryKey)
        {
            if (primaryKey == null) return "";
            return $",\nCONSTRAINT {primaryKey.ConstraintName} PRIMARY KEY({string.Join(",", primaryKey.ColumnNames)})";
        }

        public static string CreateColumn(SchemaColumn schemaColumn)
        {

            if (schemaColumn.Is_generated == "ALWAYS")
                return CreateGeneratedStoredColumn(schemaColumn);


            StringBuilder createColumnStr = new StringBuilder();
            
            var newDataType = TypesFromMSSQLtoPostgresql.GetDataTypeFromPostgresqlToMSSQL(schemaColumn.Data_type);
            createColumnStr.Append($"{schemaColumn.Column_name} {newDataType}");
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
            if (schemaColumn.Is_identity == "YES") createColumnStr.Append(CreateIdentityForColumn(schemaColumn));
            return createColumnStr.ToString();
        }

/*        private static string DefaultValueStringGenerater(SchemaColumn schemaColumn)
        {
            var defaultColumn = schemaColumn.Column_default;
            var defaultIsNull = string.IsNullOrEmpty(defaultColumn);
            if (defaultIsNull) return string.Empty;

            var defaultCaseHasCast = defaultColumn.Contains("::");
            if (defaultCaseHasCast)
            {
                string[] separator = { "::" };
                var separatedDefaultString = defaultColumn.Split(separator, StringSplitOptions.None);
                var value = separatedDefaultString[0];
                var type = separatedDefaultString[1];

                var newType = TypesFromMSSQLtoPostgresql.GetDataTypeFromPostgresqlToMSSQL(type);



                var resDefaultString = $"CAST ({value} AS {newType})";

                
            }

        }*/

        private static string CreateGeneratedStoredColumn(SchemaColumn schemaColumn)
        {
            return $"{schemaColumn.Column_name} AS {schemaColumn.Generation_expression}";
        }

        private static string CreateIdentityForColumn(SchemaColumn schemaColumn)
        {
            var identityString =
                $" IDENTITY ({schemaColumn.Identity_start}, {schemaColumn.Identity_increment}) ";
            return identityString;

        }
    }
}

