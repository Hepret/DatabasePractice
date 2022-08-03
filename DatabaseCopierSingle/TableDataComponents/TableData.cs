﻿using DatabaseCopierSingle.DatabaseTableComponents;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DatabaseCopierSingle.TableDataComponents;

namespace DatabaseCopierSingle
{
   
    class TableData : IEnumerable
    {
        public List<DataRowInterval> Data { get; private set; }
        public SchemaTable TableSchema { get; private set; }
        public int AmountOfRows
        {
            get
            {
                int res = 0;
                foreach (var dataRows in Data)
                {
                    res += dataRows.Count;
                }
                return res;
            }
        }
        public DataRowInterval this[int index]
        {
            get => Data[index];
        }

        public TableData()
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

            var length = dataInterval.Count();
            var counter = 0; // counts how many rows have been used
            
            while (length != 0)
            {
                var lastDataInterval = Data.Last();

                var freeSpace = DataRowInterval.MaxRows - lastDataInterval.Count;
                if (length <= freeSpace)
                {
                    length = 0;
                    lastDataInterval.AddRow(dataInterval.Skip(counter));
                }
                else
                {
                    Data.Add(new DataRowInterval());
                    lastDataInterval.AddRow(dataInterval.Skip(counter).Take(freeSpace));
                    lastDataInterval = Data.Last();
                    counter += freeSpace;
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
