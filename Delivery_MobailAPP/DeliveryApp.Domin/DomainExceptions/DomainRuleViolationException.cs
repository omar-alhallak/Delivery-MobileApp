using System;

namespace DeliveryApp.Domain.DomainExceptions
{
    public sealed class DomainRuleViolationException : DomainException
    {
        public DomainRuleViolationException(string code, string message) : base(code, message) { }
    }
}