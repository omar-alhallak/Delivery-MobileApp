using DeliveryApp.Domain.Entities.Merchants.Catalog;

namespace DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces
{
    public interface IMerchantCatalogCommandRepository // عقد أوامر الكتالوج: إنشاء وتعديل وحذف
    {
        Task<SystemCategory?> GetSystemCategoryAsync(SystemCategoryID id, CancellationToken ct = default);
        Task<MerchantSystemCategory?> GetMerchantSystemCategoryAsync(MerchantID merchantId, SystemCategoryID systemCategoryId, CancellationToken ct = default);
        Task<MerchantCategory?> GetMerchantCategoryAsync(MerchantCategoryID id, CancellationToken ct = default);
        Task<Product?> GetProductAsync(ProductID id, CancellationToken ct = default);
        Task<Variant?> GetVariantAsync(VariantID id, CancellationToken ct = default);
        Task<IReadOnlyList<Product>> GetProductsByCategoryAsync(MerchantCategoryID categoryId, CancellationToken ct = default);
        Task<IReadOnlyList<Variant>> GetVariantsByProductAsync(ProductID productId, CancellationToken ct = default);
        Task<IReadOnlyList<Variant>> GetVariantsByProductIdsAsync(IEnumerable<ProductID> productIds, CancellationToken ct = default);

        Task AddSystemCategoryAsync(SystemCategory category, CancellationToken ct = default);
        Task AddMerchantSystemCategoryAsync(MerchantSystemCategory link, CancellationToken ct = default);
        Task AddMerchantCategoryAsync(MerchantCategory category, CancellationToken ct = default);
        Task AddProductAsync(Product product, CancellationToken ct = default);
        Task AddVariantAsync(Variant variant, CancellationToken ct = default);

        void RemoveSystemCategory(SystemCategory category);
        void RemoveMerchantSystemCategory(MerchantSystemCategory link);
        void RemoveMerchantCategory(MerchantCategory category);
        void RemoveProduct(Product product);
        void RemoveVariant(Variant variant);
        void RemoveProducts(IEnumerable<Product> products);
        void RemoveVariants(IEnumerable<Variant> variants);

        Task SaveChangesAsync(CancellationToken ct = default);
    }
}