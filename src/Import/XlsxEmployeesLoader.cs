using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MilkmenUnion.Controllers
{
    /// <summary>
    /// Disclaimer: due to time constraints we choose to ignore possible big files and memory issues.
    /// We should come back to this and add some limits
    /// </summary>
    public class XlsxEmployeesLoader
    {
        public XlsxEmployeesLoader()
        {
            
        }

        public Task<DataLoadResult> Load(Stream file, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}