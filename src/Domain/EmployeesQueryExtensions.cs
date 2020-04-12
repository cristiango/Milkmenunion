using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MilkmenUnion.Domain
{
    public static class EmployeesQueryExtensions
    {
        private const int DefaultPageSize = 10;

        public static async Task<(IReadOnlyEmployee[] PagedEmployees, int TotalCount)> FilterProperties(
            this IQueryable<IReadOnlyEmployee> query,
            string filter,
            int? page,
            int? pageSize,
            CancellationToken cancellationToken = default)
        {
            pageSize ??= DefaultPageSize;
            if (!page.HasValue)
            {
                var totalCount = await query.CountAsync(cancellationToken);
                return (await query
                    .OrderBy(p => p.LastName)
                    .ThenBy(p => p.FistName)
                    .Take(pageSize.Value)
                    .ToArrayAsync(cancellationToken), totalCount);
            }

            

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(x =>
                    x.LastName != null && x.LastName.StartsWith(filter) ||
                    x.FistName!=null && x.FistName.StartsWith(filter)
                    );
            }

            //need to change it to count async later
            var count = query.Count();

            query = query
                .OrderBy(p => p.LastName)
                .ThenBy(p => p.FistName)
                .Skip((page.Value - 1) * pageSize.Value)
                .Take(pageSize.Value);

            //need to change it to to array async later
            IReadOnlyEmployee[] employees = query.ToArray();
            return (employees, count);
        }
    }
}