using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;
using MerchantCategoryID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantCategoryTag>;
using ProductID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.ProductTag>;

namespace DeliveryApp.Infrastructure.Implementation.MerchantCatalogRepositories
{
    public sealed class MerchantCatalogReadRepository : IMerchantCatalogReadRepository // تنفيذ قراءة الكتالوج من قاعدة البيانات
    {
        private readonly ApplicationDbContext _context; // DbContext الخاص بـ Entity Framework

        public MerchantCatalogReadRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyList<SystemCategory>> GetSystemCategoriesAsync(CancellationToken ct = default)
        {
            return await _context.SystemCategories
                .AsNoTracking()
                .OrderBy(x => x.MerchantType)
                .ThenBy(x => x.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<SystemCategory>> GetSystemCategoriesByMerchantAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return await _context.MerchantSystemCategories
                .AsNoTracking()
                .Where(x => x.MerchantID == merchantId)
                .Join(
                    _context.SystemCategories.AsNoTracking(),
                    link => link.SystemCategoryID,
                    category => category.ID,
                    (link, category) => category)
                .OrderBy(x => x.MerchantType)
                .ThenBy(x => x.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<MerchantCategory>> GetMerchantCategoriesAsync(MerchantID merchantId, bool activeOnly, CancellationToken ct = default)
        {
            var query = _context.MerchantCategories
                .AsNoTracking()
                .Where(x => x.MerchantID == merchantId);

            if (activeOnly) query = query.Where(x => x.IsActive);

            return await query
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(MerchantCategoryID categoryId, bool activeOnly, CancellationToken ct = default)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(x => x.MerchantCategoryID == categoryId);

            if (activeOnly) query = query.Where(x => x.IsActive);

            return await query
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Variant>> GetVariantsByProductAsync(ProductID productId, bool activeOnly, CancellationToken ct = default)
        {
            var query = _context.Variants
                .AsNoTracking()
                .Where(x => x.ProductID == productId);

            if (activeOnly) query = query.Where(x => x.IsActive);

            return await query
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Product>> GetProductsByCategoryIdsAsync(IEnumerable<MerchantCategoryID> categoryIds, bool activeOnly, CancellationToken ct = default)
        {
            var ids = categoryIds.Select(x => x.Value).ToArray();
            if (ids.Length == 0) return [];

            var strongIds = ids.Select(StrongID<MerchantCategoryTag>.From).ToArray();
            var query = _context.Products
                .AsNoTracking()
                .Where(x => strongIds.Contains(x.MerchantCategoryID));

            if (activeOnly) query = query.Where(x => x.IsActive);

            return await query
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Variant>> GetVariantsByProductIdsAsync(IEnumerable<ProductID> productIds, bool activeOnly, CancellationToken ct = default)
        {
            var ids = productIds.Select(x => x.Value).ToArray();
            if (ids.Length == 0) return [];

            var strongIds = ids.Select(StrongID<ProductTag>.From).ToArray();
            var query = _context.Variants
                .AsNoTracking()
                .Where(x => strongIds.Contains(x.ProductID));

            if (activeOnly) query = query.Where(x => x.IsActive);

            return await query
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);
        }
    }
}
