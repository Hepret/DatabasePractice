using System.Collections.Generic;
using DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema;

namespace DatabaseCopierSingle.DatabaseCopiers.DatabaseSchemaInserter
{
    public interface IInsertDatabaseSchema
    {
        void SetSchema(DatabaseSchemaCreatingScript databaseSchemaCreatingScript);

       
    }
}