using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MilkmenUnion.Domain
{
    /// <summary>
    /// Assumption that being an small company with handful of employees we don't design for concurrency.
    /// Also to keep things simple
    /// </summary>
    public class EmployeesRepository
    {
        private readonly EmployeesDbContext _employeesDbContext;

        public EmployeesRepository(EmployeesDbContext employeesDbContext)
        {
            _employeesDbContext = employeesDbContext;
        }

        public async Task<IReadOnlyEmployee> GetById(string id, CancellationToken cancellationToken = default) =>
            await _employeesDbContext.Employees.SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        public Task<(IReadOnlyEmployee[], int)> GetAllPaging(
            string filter = null,
            int? page = 1,
            int? pageSize = 10,
            CancellationToken cancellationToken = default) =>
            _employeesDbContext.Employees.FilterProperties(filter, page, pageSize, cancellationToken);

        public async Task AddNew(Employee employee) => 
            await _employeesDbContext.Employees.AddAsync(employee);

        public Task CommitChanges(CancellationToken cancellationToken) => 
            _employeesDbContext.SaveChangesAsync(cancellationToken);
    }
}