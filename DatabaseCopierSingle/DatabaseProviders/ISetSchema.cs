using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseCopierSingle.DatabaseProviders
{
    public interface ISetSchema
    {
        void SetSchema(string queryCommand);
    }
}
