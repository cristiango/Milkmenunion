using System;
using System.Collections.Generic;
using System.Linq;
using MilkmenUnion.Domain;
using MilkmenUnion.Storage;

namespace Milkmenunion.Tests
{
    public static class The
    {
        public static class Person
        {
            public static Employee John = Employee.CreateNew("1", new DateTime(1984, 05, 1));
            public static Employee Mark = Employee.CreateNew("2", new DateTime(1988,1,1));
        }

        public static IEnumerable<Employee> AllEmployees = new[] {Person.John, Person.Mark};

    }

    public static class DbInitializerExtensions
    {
        public static void SeedWithTestData(this CompanyDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Employees.Any())
            {
                return;
            }

            context.Employees.AddRange(The.AllEmployees);
            context.SaveChanges();
        }
    }
}