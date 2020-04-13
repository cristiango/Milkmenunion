using System.Collections.Generic;

namespace MilkmenUnion.Controllers.Models
{
    public class GetAllEmployeesResult
    {
        public int Total { get; set; }
        public IEnumerable<EmployeeSummary> Employees { get; set; }
    }
}