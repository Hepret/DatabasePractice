namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class CheckConstraint
    {
        public string TableName { get; set; }
        public string ConstraintName { get; set; }
        public string CheckClause { get; set; }

    }
}