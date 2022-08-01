using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class ForeignKey
    {
        public string ConstraintName { get; set; }
        public string SchemaCatalog { get; set; }
        public string TableName { get; set; }
        public string ColumnName { get; set; }
        public string ReferencedSchema { get; set; }
        public string ReferencedTable { get; set; }
        public string ReferencedColumn { get; set; }
        public string OnUpdate { get; set; }
        public string OnDelete { get; set; }

        public bool IsSelfReference()
        {
            return TableName == ReferencedTable;
        }
        public override string ToString()
        {
            return  $"\tTable name: {TableName}\n" +
                    $"\tColumn name: {ColumnName}\n" +
                    $"\tReference Table: {ReferencedTable} \n" +
                    $"\tReference Column: {ReferencedColumn} \n";
        }
    }
}
