using System;
using FluentValidation;

namespace MilkmenUnion.Controllers.Models.Validators
{
    public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequest>
    {
        public CreateEmployeeRequestValidator()
        {
            RuleFor(x => x.DateOfBirth)
                .NotEmpty()
                .GreaterThan(new DateTime(1900, 1, 1));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MaximumLength(128);
            RuleFor(x => x.FistName)
                .NotEmpty()
                .MaximumLength(128);
        }
    }
}