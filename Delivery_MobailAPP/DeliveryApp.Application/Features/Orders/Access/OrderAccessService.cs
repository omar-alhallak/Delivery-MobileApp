using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Domain.Enums.OrderEnums;

namespace DeliveryApp.Application.Features.Orders.Access
{
    public sealed class OrderAccessService
    {
        private readonly IOrderAccessRepository _repository;

        public OrderAccessService(IOrderAccessRepository repository)
        {
            _repository = repository;
        }

        public async Task EnsureCanReadAsync(Guid currentUserId, Order order, CancellationToken ct = default)
        {
            if (order.CustomerID.Value == currentUserId) return;
            await EnsureMerchantCanManageAsync(currentUserId, order, ct);
        }

        public async Task EnsureMerchantCanManageAsync(Guid currentUserId, Order order, CancellationToken ct = default)
        {
            if (!order.MerchantID.HasValue)
                throw new UnauthorizedAccessException("Order is not linked to a merchant.");

            var hasAccess = await _repository.HasActiveMerchantAccessAsync(
                UserID.From(currentUserId),
                order.MerchantID.Value,
                ct);

            if (!hasAccess)
                throw new UnauthorizedAccessException("Merchant order access denied.");
        }

        public async Task EnsureMerchantCanManageAsync(Guid currentUserId, Guid merchantId, CancellationToken ct = default)
        {
            var hasAccess = await _repository.HasActiveMerchantAccessAsync(
                UserID.From(currentUserId),
                MerchantID.From(merchantId),
                ct);

            if (!hasAccess)
                throw new UnauthorizedAccessException("Merchant order access denied.");
        }

        public async Task<CancelledByType> GetCancellationActorAsync(Guid currentUserId, Order order, CancellationToken ct = default)
        {
            if (order.CustomerID.Value == currentUserId)
                return CancelledByType.Customer;

            await EnsureMerchantCanManageAsync(currentUserId, order, ct);
            return CancelledByType.Merchant;
        }
    }
}
