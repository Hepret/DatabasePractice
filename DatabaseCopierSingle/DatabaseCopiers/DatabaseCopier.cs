using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.TableDataComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle
{
    abstract class DatabaseCopier
    {
        protected DatabaseProvider providerFrom;
        protected DatabaseProvider providerTo;
        protected SchemaDatabase schema;
        protected DatabaseData databaseData;
        public string NewDatabaseName { get; set; }
        public bool CreateNewDatabase { get; set; } = false;

        public void CopySchema()
        {
            try
            {
                GetSchema();

                CreateDatabaseIfNotCreated();
                // create script
                CreateSchemas();
                var createSchemaCommand = CreateScriptForCopyngDatabaseSchema();
                // send 
                SetSchema(createSchemaCommand);
            }
            catch (Exception ex)
            {
                throw new Exception($"Can't Copy Database", ex);
            }
        }

        protected abstract  void CreateSchemas();

        private void SetSchema(string createSchemaCommand)
        {   
            providerTo.SetSchema(createSchemaCommand);
        } 
        protected abstract string CreateScriptForCopyngDatabaseSchema();
        private void CreateDatabaseIfNotCreated()
        {
            if (CreateNewDatabase)
            {
                if (string.IsNullOrEmpty(NewDatabaseName))
                {
                    NewDatabaseName = providerFrom.GetDatabaseName() + "_copy";
                }

                providerTo.CreateNewDatabase(NewDatabaseName);

                providerTo.ChangeDatabase(NewDatabaseName);
            }
        }
        private void GetSchema()
        {
            schema = providerFrom.GetDatabaseSchema();
        }

        public void CopyData()
        {
            GetData();
            var dataInsertScripts = CreateDataInsertScripts();
            SetData(dataInsertScripts);
        }
        private void SetData(DataInsertScripts[] scriptsForAllTable)
        {
            foreach (var scriptsForOneTable in scriptsForAllTable)
            {
                providerTo.SetData(scriptsForOneTable?.Scripts);
            }
        }
        private void SetData(string[] dataInsertScripts)
        {
            providerTo.SetData(dataInsertScripts);
        }

        private void GetData()
        {
            if (schema == null) GetSchema();
            databaseData = providerFrom.GetData(schema);
        }
        
        
        protected abstract DataInsertScripts[] CreateDataInsertScripts();
    }
}
