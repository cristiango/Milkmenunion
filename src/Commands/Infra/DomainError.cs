using System;

namespace MilkmenUnion.Commands.Infra
{
    public class DomainError
    {
        public static readonly DomainError None = new DomainError();

        public string Message { get; }

        private DomainError()
        { }

        public DomainError(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message));
        }
    }
}