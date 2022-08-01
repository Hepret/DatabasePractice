using System.Collections;

namespace DatabaseCopierSingle
{
    class TableDataRow : IEnumerable
    {
        public object[] Data { get; private set; }

        public int ColumnAmmount
        {
            get
            {
                return Data.Length;
            }
        }
        public object this[int index]
        {
            get => Data[index];
            //set => rows[index];
        }

        public TableDataRow(object[] data)
        {
            Data = data;
        }

        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }
}