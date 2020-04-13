using System;

namespace MilkmenUnion.Controllers.Models
{
    public class CreateEmployeeRequest
    {
        public string FistName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int? Height { get; set; }
    }
}