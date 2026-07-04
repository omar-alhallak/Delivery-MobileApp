using DeliveryApp.Domain.Entities.Merchants.Catalog;

namespace DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces
{
    public interface IMerchantCatalogReadRepository //  قراءة الكتالوج بدون معرفة تفاصيل قاعدة البيانات
    {
        Task<IReadOnlyList<SystemCategory>> GetSystemCategoriesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<MerchantCategory>> GetMerchantCategoriesAsync(MerchantID merchantId, bool activeOnly, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(MerchantCategoryID categoryId, bool activeOnly, CancellationToken ct = default);
        Task<IReadOnlyList<Variant>> GetVariantsByProductAsync(ProductID productId, bool activeOnly, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetProductsByCategoryIdsAsync(IEnumerable<MerchantCategoryID> categoryIds, bool activeOnly, CancellationToken ct = default);
        Task<IReadOnlyList<Variant>> GetVariantsByProductIdsAsync(IEnumerable<ProductID> productIds, bool activeOnly, CancellationToken ct = default);
    }
}