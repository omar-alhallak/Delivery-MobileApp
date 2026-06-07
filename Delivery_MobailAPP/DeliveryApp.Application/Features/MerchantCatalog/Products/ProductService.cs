using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Merchants.Catalog;

namespace DeliveryApp.Application.Features.MerchantCatalog.Products
{
    public sealed class ProductService // Use case يجمع CRUD المنتجات
    {
        private readonly IMerchantCatalogReadRepository _readRepository;
        private readonly IMerchantCatalogCommandRepository _commandRepository;

        public ProductService(IMerchantCatalogReadRepository readRepository, IMerchantCatalogCommandRepository commandRepository)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
        }

        public async Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(Guid merchantCategoryId, CancellationToken ct = default) // جلب منتجات تصنيف
        {
            var products = await _readRepository.GetProductsByCategoryAsync(MerchantCategoryID.From(merchantCategoryId), activeOnly: false, ct);
            var variants = await _readRepository.GetVariantsByProductIdsAsync(products.Select(x => x.ID), activeOnly: false, ct);

            return products
                .Select(product => CatalogMapper.ToDto(product, variants.Where(x => x.ProductID == product.ID)))
                .ToList();
        }

        public async Task<ProductDto> CreateAsync(Guid merchantCategoryId, CreateProductRequest request, CancellationToken ct = default) // إنشاء منتج
        {
            if (request is null) throw new Exception("Product request is required.");

            var product = new Product(
                ProductID.New(),
                MerchantCategoryID.From(merchantCategoryId),
                request.ProductName,
                request.Description,
                request.SortOrder,
                request.ImageUrl,
                DateTimeOffset.UtcNow,
                request.BasePrice);

            await _commandRepository.AddProductAsync(product, ct);
            await _commandRepository.SaveChangesAsync(ct);

            return CatalogMapper.ToDto(product);
        }

        public async Task<ProductDto?> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken ct = default) // تعديل اسم أو سعر أو صورة المنتج
        {
            if (request is null) throw new Exception("Product update request is required.");

            var product = await _commandRepository.GetProductAsync(ProductID.From(id), ct);
            if (product is null) return null;

            product.Rename(request.ProductName);
            product.ChangeDescription(request.Description);
            product.ChangeImage(request.ImageUrl);
            product.ChangeBasePrice(request.BasePrice);
            product.ChangeSortOrder(request.SortOrder);

            await _commandRepository.SaveChangesAsync(ct);
            return CatalogMapper.ToDto(product);
        }

        public Task<bool> ActivateAsync(Guid id, CancellationToken ct = default)
            => ChangeStateAsync(id, activate: true, ct);

        public Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
            => ChangeStateAsync(id, activate: false, ct);

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) // حذف منتج
        {
            var productId = ProductID.From(id);
            var product = await _commandRepository.GetProductAsync(productId, ct);
            if (product is null) return false;

            var variants = await _commandRepository.GetVariantsByProductAsync(productId, ct);

            _commandRepository.RemoveVariants(variants);
            _commandRepository.RemoveProduct(product);
            await _commandRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<bool> ChangeStateAsync(Guid id, bool activate, CancellationToken ct) // تفعيل أو تعطيل المنتج
        {
            var product = await _commandRepository.GetProductAsync(ProductID.From(id), ct);
            if (product is null) return false;

            if (activate) product.Activate();
            else product.Deactivate();

            await _commandRepository.SaveChangesAsync(ct);
            return true;
        }
    }
}
