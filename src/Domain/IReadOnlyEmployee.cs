using System;

namespace MilkmenUnion.Domain
{
    public interface IReadOnlyEmployee
    {
        string Id { get; }
        string FistName { get; }
        string LastName { get; }
        int Height { get; }
        DateTime DateOfBirth { get; } //for simplicity we assume this is always present
    }
}