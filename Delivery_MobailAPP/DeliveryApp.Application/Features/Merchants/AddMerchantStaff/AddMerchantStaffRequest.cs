namespace DeliveryApp.Application.Features.Merchants.AddMerchantStaff
{
    public sealed record AddMerchantStaffRequest
    {
        public Guid MerchantId { get; init; }

        public string Phone { get; init; } = null!;
    }
}