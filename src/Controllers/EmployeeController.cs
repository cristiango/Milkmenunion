using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MilkmenUnion.Controllers.Models;
using MilkmenUnion.Domain;

namespace MilkmenUnion.Controllers
{
    public static class APILimits
    {
        public const int MaxResultsPerCall = 1000;
    }
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
        public async Task<IActionResult> GetAll(
            [FromQuery] string filter = null,
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10, 
            CancellationToken ct = default)
        {
            if (pageSize > APILimits.MaxResultsPerCall)
            {
                return new ObjectResult($"No more than {APILimits.MaxResultsPerCall} results per page")
                    {StatusCode = (int) HttpStatusCode.ExpectationFailed};
            }

            (IReadOnlyEmployee[] Employees, int Total) filterResult = await _employeesRepository.GetAllPaging(filter, page, pageSize, ct);
            return Ok(new GetAllEmployeesResult
            {
                Total = filterResult.Total,
                Employees = filterResult.Employees.Select(x=> EmployeeSummary.Load(x,_getUtcNow))
            });
        }

        [HttpPost]
        public async Task<IActionResult> OnboardNewEmployee([FromBody] CreateEmployeeRequest request,
            CancellationToken ct = default)
        {
            //TODO implement validation

            return BadRequest("reson");
        }
    }

    public class CreateEmployeeRequest
    {
        public string FistName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? Height { get; set; }
    }
}