namespace DeliveryApp.Application.Features.Merchants.GetWorkingHours
{
    public sealed class GetMerchantWorkingHoursResponse
    {
        public Guid MerchantId { get; set; }

        public List<MerchantWorkingHourItemResponse> Days { get; set; } = [];
    }

    public sealed class MerchantWorkingHourItemResponse
    {
        public DayOfWeek Day { get; set; }

        public string? OpenTime { get; set; }

        public string? CloseTime { get; set; }

        public bool IsClosed { get; set; }

        public bool IsOpenAllDay { get; set; }
    }
}