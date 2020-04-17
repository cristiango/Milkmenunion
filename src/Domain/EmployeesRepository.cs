using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilkmenUnion.Storage;

namespace MilkmenUnion.Domain
{
    /// <summary>
    /// Assumption that being an small company with handful of employees we don't design for concurrency.
    /// Also to keep things simple
    /// </summary>
    public class EmployeesRepository
    {
        private readonly CompanyDbContext _companyDbContext;

        public EmployeesRepository(CompanyDbContext companyDbContext)
        {
            _companyDbContext = companyDbContext;
        }

        public async Task<Employee> GetById(string id, CancellationToken cancellationToken = default) =>
            await _companyDbContext
                .Employees
                .Include(x=>x.SalaryHistory)
                .SingleOrDefaultAsync(e => e.Id == id, cancellationToken);

        public Task<(IReadOnlyEmployee[], int)> GetAllPaging(
            string filter = null,
            int? page = 1,
            int? pageSize = 10,
            CancellationToken cancellationToken = default) =>
            _companyDbContext.Employees.FilterProperties(filter, page, pageSize, cancellationToken);


        public async Task AddNew(Employee employee) => 
            await _companyDbContext.Employees.AddAsync(employee);

        public Task CommitChanges(CancellationToken cancellationToken) => 
            _companyDbContext.SaveChangesAsync(cancellationToken);
    }
}