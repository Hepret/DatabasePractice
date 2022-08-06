using System;

namespace DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents
{
    public class FullTableName
    {
        public string TableName { get; set; }
        public string SchemaCatalogName { get; set; }
        protected bool Equals(FullTableName other)
        {
            return TableName == other.TableName && SchemaCatalogName == other.SchemaCatalogName;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FullTableName) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TableName, SchemaCatalogName);
        }
        
    }
}
