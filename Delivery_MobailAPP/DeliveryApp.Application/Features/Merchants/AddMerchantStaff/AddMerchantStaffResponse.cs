namespace DeliveryApp.Application.Features.Merchants.AddMerchantStaff
{
    public sealed record AddMerchantStaffResponse
    {
        public Guid MerchantId { get; init; }

        public Guid StaffUserId { get; init; }

        public bool IsActive { get; init; }
    }
}