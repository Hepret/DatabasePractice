using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseProviders
{
    class CopyPostresqlToPostgresql : DatabaseCopier
    {
        public CopyPostresqlToPostgresql(string connectionStrongFrom, string connectionStringTo)
        {
            this.providerFrom = new PostgresqlProvider(connectionStrongFrom);
            this.providerTo = new PostgresqlProvider(connectionStringTo);
        }
        protected override DataInsertScripts[] CreateDataInsertScripts()
        {
            return CreatorScriptsForInsertData.CreateInsertDataScript(databaseData);
        }

        protected override void CreateSchemas()
        {
            foreach (var schemaName in schema.Schemas)
            {
                providerTo.CreateSchema(schemaName);
            }
        }

        protected override string CreateScriptForCopyngDatabaseSchema()
        {
            var script = CreatorScriptsFromSchemaPostgresqlToPostgresql.CreateScript(schema);
            return script;
        }
    }
}
