namespace DeliveryApp.Application.Features.Merchants.UpdateMerchant
{
    public sealed record UpdateMerchantResponse
    {
        public Guid MerchantId { get; init; }

        public string PublicId { get; init; } = null!;

        public bool IsActive { get; init; }
    }
}