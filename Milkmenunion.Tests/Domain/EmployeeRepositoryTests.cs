using System;
using System.Linq;
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
        private readonly CompanyDbContext _companyDbContext;

        public EmployeeRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<CompanyDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _companyDbContext = new CompanyDbContext(options);
            _employeesRepository = new EmployeesRepository(_companyDbContext);
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

        [Fact]
        public async Task can_have_its_salary_changed()
        {
            var employee = Employee.CreateNew("1", new DateTime(1992, 1, 1));
            var initialSalary = SalaryInformation.AdjustSalaryFor(employee, 1000, new DateTime(2020, 1, 1));
            await _employeesRepository.AddNew(employee);

            //Time to hack some things. Its 21:30
            await _companyDbContext.Salaries.AddAsync(initialSalary);
            await _companyDbContext.SaveChangesAsync();

            var actual = await _employeesRepository.GetById("1");
            actual.SalaryHistory.Count.ShouldBe(1);
            var salary = actual.SalaryHistory.Single();
            salary.MonthlyAmount.ShouldBe(1000);

        }
    }
}
