using System;
using System.Collections.Generic;

namespace MilkmenUnion.Domain
{
    /// <summary>
    /// Our immutable domain employee
    /// </summary>
    public class Employee : IReadOnlyEmployee
    {
        private Employee(string id) => Id = id;

        public static Employee CreateNew(string id, DateTime dateOfBirth) => new Employee(id)
        {
            DateOfBirth = dateOfBirth
        };

        public string Id { get; private set; }
        public string FistName { get; private set; }
        public string LastName { get; private set; }
        
        /// <summary>
        /// Height in cm
        /// </summary>
        public int Height { get; private set; }
        public DateTime DateOfBirth { get; private set; } //for simplicity we assume this is always present
        
        //TODO add calculated age based on UTCNow
        //MonthlyAmount and history of salary changes

        //TODO Add concurrency version

        /// <summary>
        /// Not a real immutable implementation here. Lets assume we implement clone later on
        /// </summary>
        public Employee WithEstimatedBirthDate(DateTime estimatedDateOfBirth)
        {
            this.DateOfBirth = estimatedDateOfBirth;
            this.DateOfBirthEstimated = true;
            return this;
        }


        public bool DateOfBirthEstimated { get; set; }

        public Employee WithFirstName(string firstName)
        {
            this.LastName = LastName;
            return this;
        }

        public Employee WithLastName(string lastName)
        {
            this.LastName = lastName;
            return this;
        }

        public Employee WithHeightInMeters(in double heightInMeters)
        {
            this.Height = (int)heightInMeters * 100;
            return this;
        }

        public IReadOnlyCollection<SalaryInformation> SalaryHistory { get; }

        //see how we can get this computed
        public double CurrentSalary { get; private set; }
    }

    /// <summary>
    /// We want to extend this later with some kind of audit. We want to know the person who approved this salary change at least
    /// </summary>
    public class SalaryInformation
    {
        /// <summary>
        /// WE assume that everyone gets the same number of payments per year.
        /// </summary>
        public double MonthlyAmount { get; private set; }
        public DateTime ChangeDate { get; private set; }

        public Employee Employee { get; private set; }
        public string EmployeeId { get; private set; }

        public int Id { get; private set; }


        public static SalaryInformation AdjustSalaryFor(Employee employee,double newMonthlySalary, DateTime changeDate)
        {
            return new SalaryInformation
            {
                Employee = employee,
                MonthlyAmount = newMonthlySalary,
                ChangeDate = changeDate
            };
        }
    }
}
