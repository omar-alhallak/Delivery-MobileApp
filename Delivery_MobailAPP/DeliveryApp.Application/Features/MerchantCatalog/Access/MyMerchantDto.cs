using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.MerchantCatalog.Access
{
    public sealed class MyMerchantDto // معلومات المطعم الذي يستطيع المستخدم إدارته
    {
        public Guid MerchantId { get; init; }
        public string MerchantName { get; init; } = null!;
        public MerchantUserRole Role { get; init; }
        public bool IsActive { get; init; }
    }
}
