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
        public int Height { get; private set; }
        public DateTime DateOfBirth { get; private set; } //for simplicity we assume this is always present
        
        //TODO add calculated age based on UTCNow
        //Salary and history of salary changes
    }
}
