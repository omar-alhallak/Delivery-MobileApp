namespace DeliveryApp.Domain.DomainExceptions
{
    public sealed class DomainRuleViolationException : DomainException // كسر قاعدة من القواعد الموضوعة
    {
        public DomainRuleViolationException(string code, string message) : base(code, message) { }
    }
}