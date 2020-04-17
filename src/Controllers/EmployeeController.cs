using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MilkmenUnion.Controllers.Models;
using MilkmenUnion.Controllers.Models.Validators;
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
        private readonly CreateEmployeeRequestValidator _createEmployeeRequestValidator;

        public EmployeeController(EmployeesRepository employeesRepository, GetUtcNow getUtcNow, CreateEmployeeRequestValidator createEmployeeRequestValidator)
        {
            _employeesRepository = employeesRepository;
            _getUtcNow = getUtcNow;
            _createEmployeeRequestValidator = createEmployeeRequestValidator;
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string filter = null,
            [FromQuery] int? page = 1,
            [FromQuery] int? pageSize = 10, 
            CancellationToken ct = default)
        {
            if (pageSize > ApiLimits.MaxResultsPerCall)
            {
                return new ObjectResult($"No more than {ApiLimits.MaxResultsPerCall} results per page")
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
            //do not use here ImportEmployeeHandler. That is meant only for importing.
            //For this API we should create another CreateEmployeeHandler that will have different rules perhaps
            var validationResult = await _createEmployeeRequestValidator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            return Created("dummy", null);
        }
    }
}