using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle
{
    class TableDataRows
    {
        readonly TableDataRow[] rows;
        public int Count
        {
            get => rows.Length;
        }
        public TableDataRows(TableDataRow[] rows)
        {
            this.rows = rows;
        }

        public TableDataRow this[int index]
        {
            get => rows[index];
        }
    }
}
