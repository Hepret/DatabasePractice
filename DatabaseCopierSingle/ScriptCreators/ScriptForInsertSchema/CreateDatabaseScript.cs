namespace DatabaseCopierSingle.ScriptCreators.ScriptForInsertSchema
{
    public class CreateDatabaseScript
    {
        public CreateDatabaseScript(string databaseName)
        {
            DatabaseName = databaseName;
        }

        public string DatabaseName { get; }
        public string Script { get; set; } = string.Empty;


    }
}