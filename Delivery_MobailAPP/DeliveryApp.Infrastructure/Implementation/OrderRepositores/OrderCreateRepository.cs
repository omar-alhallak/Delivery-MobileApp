using Microsoft.EntityFrameworkCore;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Domain.Entities.Customers.Orders;
using DeliveryApp.Application.Features.Orders.CreateOrder;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using AddressID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AddressTag>;
using ProductID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ProductTag>;
using VariantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.VariantTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Implementation.OrderRepositores
{
    public sealed class OrderCreateRepository : IOrderCreateRepository // تنفيذ التخزين الخاص بإنشاء الطلب
    {
        private readonly ApplicationDbContext _context; // DbContext هو بوابة التعامل مع SQL Server

        public OrderCreateRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddOrderAsync(Order order, CancellationToken ct = default) // إضافة الطلب إلى Change Tracker
        {
            await _context.Orders.AddAsync(order, ct);
        }

        public Task<bool> IsActiveMerchantAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return _context.Merchants
                .AsNoTracking()
                .AnyAsync(x => x.ID == merchantId && x.IsActive, ct);
        }

        public Task<bool> IsCompletedCustomerAddressAsync(UserID customerId, AddressID addressId, CancellationToken ct = default)
        {
            return _context.Addresses
                .AsNoTracking()
                .AnyAsync(x => x.ID == addressId
                    && x.UserID == customerId
                    && x.IsActive
                    && !x.IsTemporary,
                    ct);
        }

        public async Task<OrderCreationLocationSnapshot?> GetLocationAsync(
            UserID customerId,
            MerchantID merchantId,
            AddressID addressId,
            CancellationToken ct = default)
        {
            var merchant = await _context.Merchants
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.ID == merchantId, ct);

            if (merchant is null) return null;

            var address = await _context.Addresses
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.ID == addressId && x.UserID == customerId, ct);

            if (address is null) return null;

            return new OrderCreationLocationSnapshot(
                merchant.Location.Latitude,
                merchant.Location.Longitude,
                address.Location.Latitude,
                address.Location.Longitude);
        }

        public async Task<OrderCatalogItemSnapshot?> GetCatalogItemAsync(MerchantID merchantId, ProductID productId, VariantID? variantId, CancellationToken ct = default)
        {
            var product = await (
                from item in _context.Products.AsNoTracking()
                join category in _context.MerchantCategories.AsNoTracking()
                    on item.MerchantCategoryID equals category.ID
                join merchant in _context.Merchants.AsNoTracking()
                    on category.MerchantID equals merchant.ID
                where item.ID == productId
                    && category.MerchantID == merchantId
                    && item.IsActive
                    && category.IsActive
                    && merchant.IsActive
                select new
                {
                    Name = item.ProductName,
                    item.BasePrice
                })
                .SingleOrDefaultAsync(ct);

            if (product is null) return null;

            if (!variantId.HasValue)
            {
                return product.BasePrice.HasValue
                    ? new OrderCatalogItemSnapshot(product.Name.Value, null, product.BasePrice.Value)
                    : null;
            }

            var variant = await _context.Variants
                .AsNoTracking()
                .Where(x => x.ID == variantId.Value && x.ProductID == productId && x.IsActive)
                .Select(x => new { Name = x.VariantName, x.BasePrice })
                .SingleOrDefaultAsync(ct);

            return variant is null
                ? null
                : new OrderCatalogItemSnapshot(product.Name.Value, variant.Name.Value, variant.BasePrice);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default) // تنفيذ INSERT فعلياً بقاعدة البيانات
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}