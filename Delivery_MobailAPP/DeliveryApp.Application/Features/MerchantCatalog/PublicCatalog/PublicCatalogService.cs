using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.PublicCatalog
{
    public sealed class PublicCatalogService // Use case عرض منتجات المطعم للزبون
    {
        private readonly IMerchantCatalogReadRepository _repository; // قراءة الكتالوج فقط

        public PublicCatalogService(IMerchantCatalogReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<MerchantCatalogDto> GetMerchantCatalogAsync(Guid merchantId, CancellationToken ct = default) // جلب التصنيفات والمنتجات الفعالة فقط
        {
            var merchantStrongId = MerchantID.From(merchantId);
            var categories = await _repository.GetMerchantCategoriesAsync(merchantStrongId, activeOnly: true, ct);
            var products = await _repository.GetProductsByCategoryIdsAsync(categories.Select(x => x.ID), activeOnly: true, ct);
            var variants = await _repository.GetVariantsByProductIdsAsync(products.Select(x => x.ID), activeOnly: true, ct);

            return new MerchantCatalogDto
            {
                MerchantId = merchantId,
                Categories = categories.Select(category => new MerchantCatalogCategoryDto
                {
                    Id = category.ID.Value,
                    CategoryName = category.CategoryName.Value,
                    Description = category.Description,
                    ImageUrl = category.ImageUrl,
                    SortOrder = category.SortOrder,
                    Products = products
                        .Where(product => product.MerchantCategoryID == category.ID)
                        .Select(product => CatalogMapper.ToDto(product, variants.Where(variant => variant.ProductID == product.ID)))
                        .ToList()
                }).ToList()
            };
        }
    }
}
