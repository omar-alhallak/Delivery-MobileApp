namespace DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces
{
    public interface IOrderAccessRepository
    {
        Task<bool> HasActiveMerchantAccessAsync(UserID userId, MerchantID merchantId, CancellationToken ct = default);
    }
}
