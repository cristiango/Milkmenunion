using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MilkmenUnion.Domain
{
    public static class EmployeesQueryExtensions {
        public static async Task<(IReadOnlyEmployee PagedEmployees, int TotalCount)> FilterProperties(
            this IQueryable<IReadOnlyEmployee> employeesQuery,
            string filter,
            int? page,
            int? pageSize,
            CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}