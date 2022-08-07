using DatabaseCopierSingle.DatabaseTableComponents;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;

namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema
{
    public class CreateTableScript
    {
        private readonly SchemaTable _schemaTable;
        public FullTableName  FullTableName => _schemaTable.FullTableName;
        public string Script { get; set; }

        public CreateTableScript(SchemaTable schemaTable)
        {
            _schemaTable = schemaTable;
        }
        
        
    }
}