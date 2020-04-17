using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MilkmenUnion.Import
{
    public class JsonEmployeesLoader
    {
        public Task<DataLoadResult> Load(Stream file, CancellationToken ct = default)
        {
            throw new NotImplementedException("No time now to implement this");
        }
    }
}