namespace DeliveryApp.Application.Features.Merchants.SetWorkingHours
{
    public sealed record SetMerchantWorkingHoursRequest
    {
        public Guid MerchantId { get; init; }

        public List<WorkingHourItem> Days { get; init; } = [];
    }

    public sealed record WorkingHourItem
    {
        public DayOfWeek Day { get; init; }

        public string? OpenTime { get; init; }
        public string? CloseTime { get; init; }

        public bool IsClosed { get; init; }
        public bool IsOpenAllDay { get; init; }
    }
}