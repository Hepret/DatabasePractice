using System;
using System.Collections.Generic;
using System.Data.Common;
using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaReceivers
{
    public abstract class DatabaseSchemaReceiver : IDatabaseSchemaReceiver
    {
        protected readonly DatabaseProvider Provider;

        protected DatabaseSchemaReceiver(DatabaseProvider provider)
        {
            Provider = provider;
        }

        public SchemaDatabase GetDatabaseSchema()
        {
            try
            {
                var schemasNames = GetSchemasNames();
                var tables = GetTables();
                var sequences = GetSequences();
                SchemaDatabase schema = new SchemaDatabase(
                    schemas: schemasNames,
                    databaseName: Provider.DatabaseName,
                    tables: tables,
                    sequences: sequences
                    ) ; 
                return schema;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get schema: {Provider.DatabaseName}", ex);
            }
        }
        protected abstract List<string> GetSchemasNames();
        private List<SchemaTable> GetTables()
        {
            List<FullTableName> tableNames = GetTableNames();
            List<SchemaTable> tables = new List<SchemaTable>();
            foreach (var tableName in tableNames)
            {
                tables.Add(
                    GetTable(tableName)
                    );
            }
            return tables;
        }
        private SchemaTable GetTable(FullTableName tableName)
        {
            try
            {
                var columns =  GetTableColumns(tableName);
                var foreignKeys = GetForeignKeys(tableName);
                var primaryKey = GetPrimaryKey(tableName);
                var uniqueConstraints = GetUniques(tableName);
                var checkConstraints = GetCheckConstraints(tableName);

                SchemaTable table = new SchemaTable(
                    schemaCatalog: tableName.SchemaCatalogName,
                    tableName: tableName.TableName,
                    columns: columns,
                    foreignKeys: foreignKeys,
                    primaryKey: primaryKey,
                    uniqueConstraints: uniqueConstraints,
                    checkConstraints: checkConstraints
                    );
                return table;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get table: {tableName}", ex);
            }

        }
        protected abstract List<SchemaSequence> GetSequences();

        private List<UniqueConstraint> GetUniques(FullTableName tableName)
        {
            List<string> uniqueNames = GetUniqueConstraintNames(tableName);

            List<UniqueConstraint> uniques = new List<UniqueConstraint>();
            try
            {

                foreach (var uniqueName in uniqueNames)
                {
                    uniques.Add(GetUnique(uniqueName, tableName));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get Uniques constraint for table: {tableName}", ex);
            }
            return uniques;
        }
        protected abstract UniqueConstraint GetUnique(string uniqueName, FullTableName tableName);
        protected abstract List<string> GetUniqueConstraintNames(FullTableName tableName);
        protected abstract List<ForeignKey> GetForeignKeys(FullTableName tableName);
        protected abstract PrimaryKey GetPrimaryKey(FullTableName tableName);
        protected abstract List<SchemaColumn> GetTableColumns(FullTableName tableName);
        protected abstract List<FullTableName> GetTableNames();
        protected abstract List<CheckConstraint> GetCheckConstraints(FullTableName tableName);
        protected abstract SchemaColumn GetSchemaColumn(DbDataReader reader);
    }
}