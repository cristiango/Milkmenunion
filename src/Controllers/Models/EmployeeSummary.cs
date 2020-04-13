using System;
using MilkmenUnion.Domain;

namespace MilkmenUnion.Controllers.Models
{
    public class EmployeeSummary
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public int Height { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int Age { get; set; }
        //Salary information or maybe with different API
        public static EmployeeSummary Load(IReadOnlyEmployee employee, GetUtcNow nowUtc)
        {
            return new EmployeeSummary()
            {
                Id = employee.Id,
                DateOfBirth = employee.DateOfBirth,
                Height = employee.Height,
                Age = employee.DateOfBirth.GetAge(nowUtc()) //Another way might be that we calculate this from the data storage. But put some restriction on how we sore data
            };
        }
    }
}