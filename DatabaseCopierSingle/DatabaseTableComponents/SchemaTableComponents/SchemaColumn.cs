using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class SchemaColumn
    {
        public string Table_catalog { get; set; } 
        public string Table_schema { get; set; } 
        public string Table_name { get; set; } 
        public string Column_name { get; set; } 
        public string Ordinal_position { get; set; } 
        public string Column_default { get; set; } 
        public string Is_nullable { get; set; } 
        public string Data_type { get; set; } 
        public string Character_maximum_length { get; set; } 
        public string Character_octet_length { get; set; } 
        public string Numeric_presicion { get; set; } 
        public string Numeric_presicion_radix { get; set; } 
        public string Numeric_scale { get; set; } 
        public string Datetime_presicion { get; set; } 
        public string Character_set_catalog { get; set; } 
        public string Character_set_schema { get; set; } 
        public string Character_set_name { get; set; } 
        public string Collation_catalog { get; set; } 
        public string Is_self_referencing { get; set; }
        public string Is_identity { get; set; }
        public string Identity_generation { get; set; }
        public string Identity_start { get; set; }
        public string Identity_increment { get; set; }
        public string Identity_maximum { get; set; }
        public string Identity_minimum { get; set; }
        public string Identity_Cycle { get; set; }
        public string Is_generated { get; set; }
        public string Generation_expression { get; set; }
    }
}
