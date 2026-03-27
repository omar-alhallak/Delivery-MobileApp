namespace DeliveryApp.Domain.DomainExceptions
{
    public sealed class DomainConflictException : DomainException // يوجد تعارض (تكرار الخخ)د
    {
        public DomainConflictException(string code, string message) : base(code, message) { }
    }
}