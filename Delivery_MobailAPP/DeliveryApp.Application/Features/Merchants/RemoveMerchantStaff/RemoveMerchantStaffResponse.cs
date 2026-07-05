namespace DeliveryApp.Application.Features.Merchants.RemoveMerchantStaff
{
    public sealed record RemoveMerchantStaffResponse
    {
        public Guid MerchantId { get; init; }

        public Guid StaffUserId { get; init; }

        public bool IsActive { get; init; }
    }
}