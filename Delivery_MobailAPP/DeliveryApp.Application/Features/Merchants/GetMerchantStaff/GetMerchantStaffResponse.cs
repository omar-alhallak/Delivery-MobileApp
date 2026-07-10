using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.Merchants.GetMerchantStaff
{
    public sealed class GetMerchantStaffResponse
    {
        public Guid MerchantId { get; set; }

        public List<MerchantStaffItemResponse> Staff { get; set; } = [];
    }

    public sealed class MerchantStaffItemResponse
    {
        public Guid UserId { get; set; }

        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? PhotoUrl { get; set; }

        public MerchantUserRole Role { get; set; }

        public bool IsActive { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}