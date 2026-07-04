using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;
using SystemCategoryID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.SystemCategoryTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;
using MerchantCategoryID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantCategoryTag>;
using ProductID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ProductTag>;
using VariantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.VariantTag>;

namespace DeliveryApp.Infrastructure.Implementation.MerchantCatalogRepositories
{
    public sealed class MerchantCatalogCommandRepository : IMerchantCatalogCommandRepository // تنفيذ أوامر الكتالوج
    {
        private readonly ApplicationDbContext _context; // DbContext الذي يتابع التغييرات ويحفظها

        public MerchantCatalogCommandRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<SystemCategory?> GetSystemCategoryAsync(SystemCategoryID id, CancellationToken ct = default)
            => await _context.SystemCategories.FindAsync([id], ct);

        public async Task<MerchantSystemCategory?> GetMerchantSystemCategoryAsync(MerchantID merchantId, SystemCategoryID systemCategoryId, CancellationToken ct = default)
            => await _context.MerchantSystemCategories.FindAsync([merchantId, systemCategoryId], ct);

        public async Task<MerchantCategory?> GetMerchantCategoryAsync(MerchantCategoryID id, CancellationToken ct = default)
            => await _context.MerchantCategories.FindAsync([id], ct);

        public async Task<Product?> GetProductAsync(ProductID id, CancellationToken ct = default)
            => await _context.Products.FindAsync([id], ct);

        public async Task<Variant?> GetVariantAsync(VariantID id, CancellationToken ct = default)
            => await _context.Variants.FindAsync([id], ct);

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(MerchantCategoryID categoryId, CancellationToken ct = default)
        {
            return await _context.Products
                .Where(x => x.MerchantCategoryID == categoryId)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Variant>> GetVariantsByProductAsync(ProductID productId, CancellationToken ct = default)
        {
            return await _context.Variants
                .Where(x => x.ProductID == productId)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Variant>> GetVariantsByProductIdsAsync(IEnumerable<ProductID> productIds, CancellationToken ct = default)
        {
            var ids = productIds.Select(x => x.Value).ToArray();
            if (ids.Length == 0) return [];

            var strongIds = ids.Select(StrongID<ProductTag>.From).ToArray();
            return await _context.Variants
                .Where(x => strongIds.Contains(x.ProductID))
                .ToListAsync(ct);
        }

        public async Task AddSystemCategoryAsync(SystemCategory category, CancellationToken ct = default)
            => await _context.SystemCategories.AddAsync(category, ct);

        public async Task AddMerchantSystemCategoryAsync(MerchantSystemCategory link, CancellationToken ct = default)
            => await _context.MerchantSystemCategories.AddAsync(link, ct);

        public async Task AddMerchantCategoryAsync(MerchantCategory category, CancellationToken ct = default)
            => await _context.MerchantCategories.AddAsync(category, ct);

        public async Task AddProductAsync(Product product, CancellationToken ct = default)
            => await _context.Products.AddAsync(product, ct);

        public async Task AddVariantAsync(Variant variant, CancellationToken ct = default)
            => await _context.Variants.AddAsync(variant, ct);

        public void RemoveSystemCategory(SystemCategory category)
            => _context.SystemCategories.Remove(category);

        public void RemoveMerchantSystemCategory(MerchantSystemCategory link)
            => _context.MerchantSystemCategories.Remove(link);

        public void RemoveMerchantCategory(MerchantCategory category)
            => _context.MerchantCategories.Remove(category);

        public void RemoveProduct(Product product)
            => _context.Products.Remove(product);

        public void RemoveVariant(Variant variant)
            => _context.Variants.Remove(variant);

        public void RemoveProducts(IEnumerable<Product> products)
            => _context.Products.RemoveRange(products);

        public void RemoveVariants(IEnumerable<Variant> variants)
            => _context.Variants.RemoveRange(variants);

        public async Task SaveChangesAsync(CancellationToken ct = default)
            => await _context.SaveChangesAsync(ct);
    }
}