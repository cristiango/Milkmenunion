using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilkmenUnion.Commands.Infra;
using MilkmenUnion.Domain;
using Polly;

namespace MilkmenUnion.Commands
{
    public class ImportEmployeeHandler
    {
        /// <summary>
        /// We need to generate an employee ID in a deterministic fashion.
        /// In this way we can import same employee multiple times without side effects.
        /// </summary>
        private static readonly DeterministicGuidGenerator EmployeeIdGenerator =
            new DeterministicGuidGenerator(Guid.Parse("6BFA7CB3-8EBF-4617-9BF7-BB1145842055"));

        private const int CommandHandleMaxRetries = 3; //Change this to come from IConfiguration
        private readonly ILogger<ImportEmployeeHandler> _logger;
        private readonly EmployeesRepository _employeesRepository;
        private readonly GetUtcNow _getUtcNow;

        public ImportEmployeeHandler(ILogger<ImportEmployeeHandler> logger, EmployeesRepository employeesRepository, GetUtcNow getUtcNow)
        {
            _logger = logger;
            _employeesRepository = employeesRepository;
            _getUtcNow = getUtcNow;
        }

        public async Task<(CommandHandleStatus status, IReadOnlyEmployee employee)> Handle(
            ImportEmployee command, CancellationToken ct = default)
        {
            //Simple but effective retry policy. Having an http endpoint exposes us to concurrency issues.
            //Or is there a glitch in the network?
            var retryPolicy = Policy
                .Handle<DbUpdateException>()
                .RetryAsync(CommandHandleMaxRetries,
                    (exception, i) =>
                    {
                        _logger.LogDebug(exception,
                            $"Failed to import employee due to concurrency. Retry {i} of {CommandHandleMaxRetries}");
                    });

            var policyResult =
                await retryPolicy.ExecuteAndCaptureAsync(() => ImportEmployee(command, ct));

            return HandleResult(policyResult);
        }

        private async Task<(Employee NewEmployee, DomainError DomainError)> ImportEmployee(ImportEmployee command,
            CancellationToken cancellationToken)
        {
            //We can predetermine our employee ID based on FistName + LastName + Age + Height.
            var employeeId = EmployeeIdGenerator.Create(Encoding.UTF8.GetBytes(command.ToUniqueIdentifier)).ToString();

            var employee = await _employeesRepository.GetById(employeeId, cancellationToken);
            if (employee != null)
            {
                return (employee, DomainError.None);
            }

            
            if (command.Age < 18) //lets assume minimum working age is 18
            {
                //hey we are doing something doggy here
                return (null, new DomainError("Child labor long time ago abolished. There must be a mistake somewhere"));
            }
            //we wont check for now the max life limit

            //This gets interesting since we don't have the actual date of birth. but our company needs it to send a birthday postcard every year
            employee = 
                Employee.CreateNew(employeeId, _getUtcNow())
                //should be good approximation. Anyway we will mark that the real birthday is not supplied and later on in UI someone will notice
                .WithEstimatedBirthDate(_getUtcNow().AddYears(-command.Age)) 
                .WithFirstName(command.FirstName)
                .WithLastName(command.LastName)
                .WithHeightInMeters(command.HeightInMeters);

            await _employeesRepository.AddNew(employee);
            await _employeesRepository.CommitChanges(cancellationToken);

            _logger.LogDebug("Employee {displayName} imported", $"{command.FirstName} {command.LastName}");
            return (employee, DomainError.None);
        }

        private static (CommandHandleStatus CommandHandleStatus, IReadOnlyEmployee NewProfile) HandleResult(
            PolicyResult<(Employee NewProfile, DomainError DomainError)> policyResult)
        {
            CommandHandleStatus status;
            if (policyResult.Outcome == OutcomeType.Successful)
            {
                if (policyResult.Result.DomainError == DomainError.None)
                {
                    return (CommandHandleStatus.Successs, policyResult.Result.NewProfile);
                }

                //something we know how to handle not transient
                status = new CommandHandleStatus(
                    CommandErrorType.Domain,
                    $"Failed to create employee. {policyResult.Result.DomainError.Message}",
                    domainError: policyResult.Result.DomainError);
            }
            else
            {
                //unknown db exceptions
                status = new CommandHandleStatus(
                    CommandErrorType.System,
                    $"Failed to create employee. {policyResult.FinalException.Message}",
                    systemError: policyResult.FinalException.ToString());
            }

            return (status, default);
        }

    }

    public class ImportEmployee
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public double HeightInMeters { get; set; }

        public string ToUniqueIdentifier => $"{FirstName}##{LastName}##{HeightInMeters}##{Age}";
    }
}