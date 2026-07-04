using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Application.Features.Orders.CreateOrder;

namespace DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces
{
    public interface IOrderCreateRepository // عقد الإنشاء الذي يعزل Application عن Entity Framework
    {
        Task AddOrderAsync(Order order, CancellationToken ct = default); // تجهيز الطلب للإضافة
        Task<bool> IsActiveMerchantAsync(MerchantID merchantId, CancellationToken ct = default);
        Task<bool> IsCompletedCustomerAddressAsync(UserID customerId, AddressID addressId, CancellationToken ct = default);
        Task<OrderCreationLocationSnapshot?> GetLocationAsync(UserID customerId, MerchantID merchantId, AddressID addressId, CancellationToken ct = default);
        Task<OrderCatalogItemSnapshot?> GetCatalogItemAsync(MerchantID merchantId, ProductID productId, VariantID? variantId, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default); // حفظ التغييرات فعلياً
    }
}