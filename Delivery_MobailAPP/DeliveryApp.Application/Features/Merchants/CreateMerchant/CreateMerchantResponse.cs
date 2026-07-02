namespace DeliveryApp.Application.Features.Merchants.CreateMerchant
{
    public sealed record CreateMerchantResponse
    {
        public Guid MerchantId { get; init; }

        public string PublicId { get; init; } = null!;

        public bool IsActive { get; init; }
    }
}