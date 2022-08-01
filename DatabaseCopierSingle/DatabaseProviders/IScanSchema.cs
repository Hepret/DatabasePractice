using DatabaseCopierSingle.DatabaseTableComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseProviders
{
    public interface IScanSchema
    {
        SchemaDatabase GetDatabaseSchema();
    }
}
