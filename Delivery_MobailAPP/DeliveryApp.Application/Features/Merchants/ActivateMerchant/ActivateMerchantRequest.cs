namespace DeliveryApp.Application.Features.Merchants.ActivateMerchant
{
    public sealed record ActivateMerchantRequest
    {
        public Guid MerchantId { get; init; }
    }
}