using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DatabaseCopierSingle
{
    public class DataInsertScripts : IEnumerable
    {
        public List<string> Scripts { get; private set; }
        public DataInsertScripts(IEnumerable<string> scripts)
        {
            Scripts = scripts.ToList();
        }
        public DataInsertScripts()
        {
            Scripts = new List<string>();
        }
        public void AddRange(IEnumerable<string> scripts)
        {
            Scripts.AddRange(scripts);
        }
        public void Add(string script)
        {
            Scripts.Add(script);
        }

        public IEnumerator GetEnumerator()
        {
            return Scripts.GetEnumerator();
        }
    }
}