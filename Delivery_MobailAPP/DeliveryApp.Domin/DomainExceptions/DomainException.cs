namespace DeliveryApp.Domain.DomainExceptions
{
    public abstract class DomainException : Exception // الأب لكل الأخطاء
    {
        public string Code { get; }

        protected DomainException(string code, string message) : base(message)
        {
            if (string.IsNullOrWhiteSpace(code)) throw new ArgumentException("Error code is required.", nameof(code));

            Code = code;
        }
    }
}