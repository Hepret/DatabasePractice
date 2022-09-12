using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseTableComponents
{
    public class SchemaSequence
    {
        public string SequenceCatalog { get; set; }
        public string SequenceSchema { get; set; }
        public string SequenceName { get; set; }
        public string DataType { get; set; }
        public string NumericPresicion { get; set; }
        public string NumericPresicionRadix { get; set; }
        public string NumericScale { get; set; }
        public string StartValue { get; set; }
        public string MinimumValue { get; set; }
        public string MaximumValue { get; set; }
        public string Increment { get; set; }
        public string CycleOption { get; set; }
        
        public object LastValue { get; set; }
    }
}
