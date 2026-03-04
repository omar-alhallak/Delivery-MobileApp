using System;

namespace DeliveryApp.Domain.DomainExceptions
{
    public sealed class DomainValidationException : DomainException
    {
        public string? Field { get; }

        public DomainValidationException(string code, string message, string? field = null) : base(code, message)
        {
            Field = field;
        }
    }
}