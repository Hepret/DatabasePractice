using System.Collections;
using System.Collections.Generic;

namespace DatabaseCopierSingle.DatabaseProviders
{
    internal interface ISendData
    {
        void SetData(string queryStringForInsertData);
        void SetData(IEnumerable<string> queryStringsForInsertData);
    }
}