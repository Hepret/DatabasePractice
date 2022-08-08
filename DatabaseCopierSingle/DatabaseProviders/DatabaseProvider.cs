using System;
using System.Data;
using System.Data.Common;

namespace DatabaseCopierSingle.DatabaseProviders
{
    public abstract class DatabaseProvider 
    {
        protected readonly DbConnection Conn;

        public string DatabaseName => Conn.Database;

        protected DatabaseProvider(DbConnection connection)
        {
            Conn = connection;
            try
            {
                Conn.Open();
            }
            catch (Exception e)
            {
                Conn.Close(); 
                throw new Exception($"Can't connect to database, with connection string {Conn.ConnectionString}", e);
            }
        }
        public void ChangeDatabase(string databaseName)
        {
            try
            {
                if (Conn.State == ConnectionState.Closed) Conn.Open();
                Conn.ChangeDatabase(databaseName);
            }
            catch (DbException e)
            {
                Conn.Close();
                throw new Exception($"Can't change database to: {databaseName}", e);
            }
        }
        public void ExecuteCommand(string command)
        {
            try
            {
                var cmd = Conn.CreateCommand();
                cmd.CommandText = command;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Conn.Close();
                throw new Exception($"Invalid Operation:\n {command}", e);
            }
            
        }
        public abstract int ExecuteCommandScalar(string command);
        public DbDataReader GetDataReader(string queryString)
        {
            try
            {
                var cmd = Conn.CreateCommand();
                cmd.CommandText = queryString;
                var reader = cmd.ExecuteReader();
                return reader;  
            }
            catch (Exception e)
            {
                Conn.Close();
                throw new Exception($"Invalid Operation:\n {queryString}", e);
            }
        }
        /*public void CreateNewDatabase(string databaseName)
        {
            var queryString = $"CREATE DATABASE {databaseName};";
            ExecuteCommand(queryString);
        }
        public string GetDatabaseName()
        {
            return Conn.Database;
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
                    databaseName: Conn.Database,
                    tables: tables,
                    sequences: sequences
                    ) ; 
                return schema;
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't get schema: {Conn.Database}", ex);
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
        // SETTING SCHEMA
        public void SetSchema(string queryStringForCreateSchema)
        {
            ExecuteCommand(queryStringForCreateSchema);
        }
         
        // GETTING DATA 
        public DatabaseData GetData(SchemaDatabase schema)
        {
            DatabaseData databaseData = new DatabaseData(schema);
            foreach (var table in databaseData.TableDatas)
            {
                // Empty Test
                var amountOfRowsInTheTable = GetNumberOfRowsInTheTable(table.TableSchema.FullTableName);
                if (amountOfRowsInTheTable == 0) continue;
                
                // Self Referencing 
                if (table.TableSchema.HasSelfReference) GetDataFromTableWithSelfReference(table, amountOfRowsInTheTable);
                
                // Usual Tables
                GetDataFromTable(table, amountOfRowsInTheTable);
            }
            return databaseData;
        }

        // TODO GetDataFromTableWithSelfReference
        private void GetDataFromTableWithSelfReference(TableData table, int amountOfRows)
        {
            throw new NotImplementedException();
            var fullTableName = table.TableSchema.FullTableName;
        }

        private void GetDataFromTable(TableData table, int amountOfRows)
        {
            var tableName = table.TableSchema.FullTableName;
            for (int i = 0; i <= amountOfRows / 100; i++)
            {
                int rowsAmountToGet = i == (amountOfRows / 100) ? amountOfRows % 100 : 100;
                var rangeOfRows = GetRangeOfRowsFromTable(tableName, i * 100, rowsAmountToGet);
                table.AddData(rangeOfRows);
            }
        }
        
        protected DbDataReader GetDataReader(string queryString)
        {
            try
            {
                var cmd = Conn.CreateCommand();
                cmd.CommandText = queryString;
                var reader = cmd.ExecuteReader();
                return reader;  
            }
            catch (Exception e)
            {
                Conn.Close();
                throw new Exception($"Invalid Operation:\n {queryString}", e);
            }
        }
        protected TableDataRow GetDataRowFromReader(DbDataReader reader)
        {
            var amountOfColumns = reader.FieldCount;
            object[] tmp = new object[amountOfColumns];
            reader.GetValues(tmp);
            TableDataRow row = new TableDataRow((object[])tmp.Clone());
            return row;
        }
        protected abstract TableDataRow[] GetRangeOfRowsFromTable(FullTableName tableName, int startWith = 0, int amountOfRows = 100);
        protected abstract int GetNumberOfRowsInTheTable(FullTableName tableName);
        public abstract void CreateSchema(string schemaName);*/
        
    }
}
