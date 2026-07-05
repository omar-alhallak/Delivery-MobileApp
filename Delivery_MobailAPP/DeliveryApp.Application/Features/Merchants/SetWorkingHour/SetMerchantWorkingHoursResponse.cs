namespace DeliveryApp.Application.Features.Merchants.SetWorkingHours
{
    public sealed record SetMerchantWorkingHoursResponse
    {
        public Guid MerchantId { get; init; }
        public int DaysCount { get; init; }
    }
}