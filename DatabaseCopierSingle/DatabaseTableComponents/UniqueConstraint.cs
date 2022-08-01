using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class UniqueConstraint
    {
        public string ConstraintName { get; set; }
        public FullTableName FullTableName { get; set; }
        public List<string> ColumnNames { get; set; }



        public override string ToString()
        {
            return $"CONSTRAINT {ConstraintName} UNIQUE ({string.Join(",", ConstraintName)})";
        }
    }
}
