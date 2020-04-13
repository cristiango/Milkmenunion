using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MilkmenUnion.Domain;
using MilkmenUnion.Storage;
using Shouldly;
using Xunit;

namespace Milkmenunion.Tests.Domain
{
    public class EmployeeRepositoryTests
    {
        private readonly EmployeesRepository _employeesRepository;

        public EmployeeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CompanyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var employeeDbContext = new CompanyDbContext(options);
            _employeesRepository = new EmployeesRepository(employeeDbContext);
        }

        [Fact]
        public async Task can_create_employee()
        {
            var longTimeAgo = new DateTime(1956,2,1);
            var newGuy = Employee.CreateNew(id:"1",dateOfBirth: longTimeAgo);

            await _employeesRepository.AddNew(newGuy);
            await _employeesRepository.CommitChanges(CancellationToken.None);

            var employee = await _employeesRepository.GetById("1");
            employee.ShouldNotBeNull();
            employee.DateOfBirth.ShouldBe(longTimeAgo);
        }
    }
}
