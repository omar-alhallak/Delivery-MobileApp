using DeliveryApp.Domain.ValueObjects;

namespace DeliveryApp.Application.Interfaces
{
    public interface IPublicCodeGenerator
    {
        Task<PublicCode> GenerateUserCodeAsync(CancellationToken ct = default);
        Task<PublicCode> GenerateOrderCodeAsync(CancellationToken ct = default);
        Task<PublicCode> GenerateMerchantCodeAsync(CancellationToken ct = default);
    }
}