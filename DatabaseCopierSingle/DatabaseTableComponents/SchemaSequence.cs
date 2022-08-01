using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class SchemaSequence
    {
        public string Sequence_catalog { get; set; }
        public string Sequence_schema { get; set; }
        public string Sequence_name { get; set; }
        public string Data_type { get; set; }
        public string Numeric_presicion { get; set; }
        public string Numeric_presicion_radix { get; set; }
        public string Numeric_scale { get; set; }
        public string Start_vlaue { get; set; }
        public string Minimum_value { get; set; }
        public string Maximum_value { get; set; }
        public string Increment { get; set; }
        public string Cycle_option { get; set; }
    }
}
