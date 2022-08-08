using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseCopierSingle.DatabaseTableComponents.SchemaTableComponents;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class PrimaryKey
    {
        public string ConstraintName { get; set; }
        public FullTableName FullTableName { get; set; }
        public List<string> ColumnNames { get; set; }

        public override string ToString()
        {
            return $"{ConstraintName} - PRIMARY KEY : {FullTableName.TableName}({string.Join(",", ConstraintName)})";
        }
    }
}
