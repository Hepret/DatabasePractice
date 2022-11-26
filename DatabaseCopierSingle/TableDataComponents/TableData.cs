using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DatabaseCopierSingle.DatabaseTableComponents;

namespace DatabaseCopierSingle.TableDataComponents
{
   
    public class TableData : IEnumerable
    {
        public List<DataRowInterval> Data { get; private set; }
        public SchemaTable TableSchema { get; private set; }
        public int AmountOfRows
        {
            get
            {
                return Data.Sum(dataRows => dataRows.Count);
            }
        }
        public DataRowInterval this[int index] => Data[index];

        private TableData()
        { 
            Data = new List<DataRowInterval>();
        }
        
        public TableData(SchemaTable schemaTable) : this()
        {
            TableSchema = schemaTable;
           
        }
        public void AddData(DataRowInterval dataRowInterval)
        {
            Data.Add(dataRowInterval);
        }

        public void AddData(TableDataRow row)
        {
            var isEmpty = !Data.Any();
            var hasMaximumRows = Data.LastOrDefault()?.Count == DataRowInterval.MaxRows;
            if (isEmpty || hasMaximumRows)
            {
                Data.Add(new DataRowInterval());
            }
            Data.Last().AddRow(row);
        }
        public void AddData(IEnumerable<TableDataRow> dataInterval)
        {
            var isEmpty = !Data.Any();
            if (isEmpty) Data.Add(new DataRowInterval());


            IEnumerable<TableDataRow> remainingRows = dataInterval;
            var remainingRowsAmount = dataInterval.Count(); 
            while (remainingRowsAmount > 0)
            {
                var currentDataInterval = Data.Last();
                var availablePlaces = DataRowInterval.MaxRows - currentDataInterval.Count;
                
                if (availablePlaces >= remainingRowsAmount)
                {
                    currentDataInterval.AddRow(remainingRows);
                    return;
                }

                else
                {
                    currentDataInterval.AddRow(remainingRows.Take(availablePlaces));
                    remainingRowsAmount -= availablePlaces;
                    remainingRows = remainingRows.Skip(availablePlaces);
                    Data.Add(new DataRowInterval());
                }

            }
        }
        public void AddData(IEnumerable<DataRowInterval> dataIntervals)
        {
            Data.AddRange(dataIntervals);
        }
        public IEnumerator GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }
    
}
