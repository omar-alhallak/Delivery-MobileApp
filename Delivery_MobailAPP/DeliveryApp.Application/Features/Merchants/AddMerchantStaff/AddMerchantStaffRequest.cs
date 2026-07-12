namespace DeliveryApp.Application.Features.Merchants.AddMerchantStaff
{
    public sealed record AddMerchantStaffRequest
    {
        public Guid MerchantId { get; init; }

        public String PublicCode { get; init; } = null!;

        public string Phone { get; init; } = null!;
    }
}