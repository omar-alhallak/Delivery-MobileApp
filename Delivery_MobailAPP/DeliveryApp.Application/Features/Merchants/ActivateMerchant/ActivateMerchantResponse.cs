namespace DeliveryApp.Application.Features.Merchants.ActivateMerchant
{
    public sealed record ActivateMerchantResponse
    {
        public Guid MerchantId { get; init; }
        public bool IsActive { get; init; }
    }
}