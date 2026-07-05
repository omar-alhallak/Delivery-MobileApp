namespace DeliveryApp.Application.Features.Merchants.RemoveMerchantStaff
{
    public sealed record RemoveMerchantStaffRequest
    {
        public Guid MerchantId { get; init; }

        public Guid StaffUserId { get; init; }
    }
}