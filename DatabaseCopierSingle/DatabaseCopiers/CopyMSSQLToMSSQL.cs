using DatabaseCopierSingle.DatabaseProviders;
using DatabaseCopierSingle.ScriptCreators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseCopiers
{
    class CopyMSSQLToMSSQL : DatabaseCopier
    {
        public CopyMSSQLToMSSQL(string connectionStrongFrom, string connectionStringTo)
        {
            this.providerFrom = new MSSQLProvider(connectionStrongFrom);
            this.providerTo = new MSSQLProvider(connectionStringTo);
        }
        protected override DataInsertScripts[] CreateDataInsertScripts()
        {
            return CreatorScriptsForInsertDataMSSQLToMSSQL.CreateInsertDataScript(databaseData);
        }

        protected override void CreateSchemas()
        {
            foreach (var schemaName in schema.Schemas)
            {
                this.providerTo.CreateSchema(schemaName);
            }
        }

        protected override string CreateScriptForCopyngDatabaseSchema()
        {
            return CreatorScriptsFromSchemaMSSQLToMSSQL.CreateScript(schema);
        }
    }
}
