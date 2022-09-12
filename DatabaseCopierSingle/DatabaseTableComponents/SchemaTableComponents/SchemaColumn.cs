namespace DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents
{
    public class SchemaColumn
    {
        public string TableCatalog { get; set; } 
        public string TableSchema { get; set; } 
        public string TableName { get; set; } 
        public string ColumnName { get; set; } 
        public string OrdinalPosition { get; set; } 
        public string ColumnDefault { get; set; } 
        public string IsNullable { get; set; } 
        public string DataType { get; set; } 
        public string CharacterMaximumLength { get; set; } 
        public string CharacterOctetLength { get; set; } 
        public string NumericPresicion { get; set; } 
        public string NumericPresicionRadix { get; set; } 
        public string NumericScale { get; set; } 
        public string DatetimePresicion { get; set; } 
        public string CharacterSetCatalog { get; set; } 
        public string CharacterSetSchema { get; set; } 
        public string CharacterSetName { get; set; } 
        public string CollationCatalog { get; set; } 
        public string IsSelfReferencing { get; set; }
        public string IsIdentity { get; set; }
        public string IdentityGeneration { get; set; }
        public string IdentityStart { get; set; }
        public string IdentityIncrement { get; set; }
        public string IdentityMaximum { get; set; }
        public string IdentityMinimum { get; set; }
        public string IdentityCycle { get; set; }
        public string IsGenerated { get; set; }
        public string GenerationExpression { get; set; }
    }
}
