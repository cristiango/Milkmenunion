using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MilkmenUnion.Controllers.Models;
using MilkmenUnion.Domain;

namespace MilkmenUnion.Controllers
{
    /// <summary>
    /// This is private level API and Authentication is out of scope of this exercise
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class EmployeeController: ControllerBase
    {
        private readonly EmployeesRepository _employeesRepository;
        private readonly GetUtcNow _getUtcNow;

        public EmployeeController(EmployeesRepository employeesRepository, GetUtcNow getUtcNow)
        {
            _employeesRepository = employeesRepository;
            _getUtcNow = getUtcNow;
        }

        [HttpGet("")]
        public async Task<GetAllEmployeesResult> GetAll(
            [FromQuery] string filter = null,
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10, 
            CancellationToken ct = default)
        {
            (IReadOnlyEmployee[] Employees, int Total) filterResult = await _employeesRepository.GetAllPaging(filter, page, pageSize, ct);
            return new GetAllEmployeesResult()
            {
                Total = filterResult.Total,
                Employees = filterResult.Employees.Select(x=> EmployeeSummary.Load(x,_getUtcNow))
            };
        }
    }
}