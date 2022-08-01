using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.TableDataComponents;
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
    abstract class DatabaseProvider : IScanSchema, ISetSchema, IScanData, ISendData
    {
        protected DbConnection conn;
        //protected SchemaDatabase schema;
        public DatabaseProvider(DbConnection connection)
        {
            conn = connection;
            try
            {
                conn.Open();
                
            }
            catch (Exception e)
            {
                conn.Close(); 
                throw new Exception($"Can't connect to database, with connection string {conn.ConnectionString}", e);
            }
        }
        public void ChangeDatabase(string databaseName)
        {
            try
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                conn.ChangeDatabase(databaseName);
            }
            catch (DbException e)
            {
                conn.Close();
                throw new Exception($"Can't change database to: {databaseName}", e);
            }
        }
        protected void ExecuteCommand(string command)
        {
            try
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = command;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                conn.Close();
                throw new Exception($"Invalid Operation:\n {command}", e);
            }
            
        }
        protected abstract int ExecuteCommandScalar(string command);
        public void CreateNewDatabase(string databaseNama)
        {
            var queryString = $"CREATE DATABASE {databaseNama};";
            ExecuteCommand(queryString);
        }
        public string GetDatabaseName()
        {
            return conn.Database;
        }

        // GETTING SCHEMA 
        public SchemaDatabase GetDatabaseSchema()
        {
            try
            {
                var schemasNames = GetSchemasNames();
                var tables = GetTables();
                var sequences = GetSequences();
                SchemaDatabase schema = new SchemaDatabase(
                    schemas: schemasNames,
                    databaseName: conn.Database,
                    tables: tables,
                    sequences: sequences
                    ) ; 
                return schema;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get schema: {conn.Database}", ex);
            }
        }
        abstract protected List<string> GetSchemasNames();
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
        protected List<UniqueConstraint> GetUniques(FullTableName tableName)
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
        // SETTING SCHEMA
        public void SetSchema(string queryStringForCreateSchema)
        {
            ExecuteCommand(queryStringForCreateSchema);
        }
        // SENDDING DATA
        public void SetData(string queryStringToInsertData)
        {
            if (queryStringToInsertData != null)
            ExecuteCommand(queryStringToInsertData);
        }
        public void SetData(IEnumerable<string> queryStringsForInsertData)
        {
            foreach (var queryString in queryStringsForInsertData)
            {
                if (queryString == null) continue;
                SetData(queryString);
            }
        }      
        // GETTING DATA 
        public DatabaseData GetData(SchemaDatabase schema)
        {
            DatabaseData databaseData = new DatabaseData(schema);
            foreach (var table in databaseData.TableDatas)
            {
                GetDataFromTable(table);
            }
            return databaseData;
        }
        protected void GetDataFromTable(TableData table)
        {
            var tableName = table.TableSchema.FullTableName;
            var ammountOfRows = GetNumberOfRowsInTheTable(tableName);
            if (ammountOfRows == 0) return;
            for (int i = 0; i <= ammountOfRows / 100; i++)
            {
                int rowsAmmountToGet = i == (ammountOfRows / 100) ? ammountOfRows % 100 : 100;
                var rangeOfRows = GetRangeOfRowsFromTable(tableName, i * 100, rowsAmmountToGet);
                table.AddData(rangeOfRows);
            }
        }
        protected DbDataReader GetDataReader(string queryString)
        {
            try
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = queryString;
                var reader = cmd.ExecuteReader();
                return reader;  
            }
            catch (Exception e)
            {
                conn.Close();
                throw new Exception($"Invalid Operation:\n {queryString}", e);
            }
        }
        protected TableDataRow GetDataRowFromReader(DbDataReader reader)
        {
            var ammountOfColumns = reader.FieldCount;
            object[] tmp = new object[ammountOfColumns];
            reader.GetValues(tmp);
            TableDataRow row = new TableDataRow((object[])tmp.Clone());
            return row;
        }
        protected abstract TableDataRows GetRangeOfRowsFromTable(FullTableName tableName, int startWith = 0, int ammountOfRows = 100);
        protected abstract int GetNumberOfRowsInTheTable(FullTableName tableName);
        public abstract void CreateSchema(string schemaName);
        
    }
}
