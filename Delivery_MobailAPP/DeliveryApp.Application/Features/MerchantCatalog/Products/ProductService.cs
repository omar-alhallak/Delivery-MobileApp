using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Features.MerchantCatalog.Access;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.Products
{
    public sealed class ProductService // Use case يجمع CRUD المنتجات
    {
        private readonly IMerchantCatalogReadRepository _readRepository;
        private readonly IMerchantCatalogCommandRepository _commandRepository;
        private readonly MerchantCatalogAccessService _accessService;

        public ProductService(IMerchantCatalogReadRepository readRepository, IMerchantCatalogCommandRepository commandRepository,
            MerchantCatalogAccessService accessService)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
            _accessService = accessService;
        }

        public async Task<IReadOnlyList<ProductDto>> GetByCategoryAsync(Guid userId, Guid merchantCategoryId, CancellationToken ct = default) // جلب منتجات تصنيف
        {
            var categoryId = MerchantCategoryID.From(merchantCategoryId);
            var category = await _commandRepository.GetMerchantCategoryAsync(categoryId, ct);
            if (category is null) return [];

            await _accessService.EnsureCanManageAsync(userId, category.MerchantID, ct);

            var products = await _readRepository.GetProductsByCategoryAsync(categoryId, activeOnly: false, ct);
            var variants = await _readRepository.GetVariantsByProductIdsAsync(products.Select(x => x.ID), activeOnly: false, ct);

            return products
                .Select(product => CatalogMapper.ToDto(product, variants.Where(x => x.ProductID == product.ID)))
                .ToList();
        }

        public async Task<ProductDto> CreateAsync(Guid userId, Guid merchantCategoryId, CreateProductRequest request, CancellationToken ct = default) // إنشاء منتج
        {
            if (request is null) throw new Exception("Product request is required.");

            var categoryId = MerchantCategoryID.From(merchantCategoryId);
            var category = await _commandRepository.GetMerchantCategoryAsync(categoryId, ct);
            if (category is null) throw new KeyNotFoundException("Merchant category not found.");

            await _accessService.EnsureCanManageAsync(userId, category.MerchantID, ct);

            var product = new Product(
                ProductID.New(),
                categoryId,
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

        public async Task<ProductDto?> UpdateAsync(Guid userId, Guid id, UpdateProductRequest request, CancellationToken ct = default) // تعديل اسم أو سعر أو صورة المنتج
        {
            if (request is null) throw new Exception("Product update request is required.");

            var product = await _commandRepository.GetProductAsync(ProductID.From(id), ct);
            if (product is null) return null;

            await EnsureCanManageProductAsync(userId, product, ct);

            product.Rename(request.ProductName);
            product.ChangeDescription(request.Description);
            product.ChangeImage(request.ImageUrl);
            product.ChangeBasePrice(request.BasePrice);
            product.ChangeSortOrder(request.SortOrder);

            await _commandRepository.SaveChangesAsync(ct);
            return CatalogMapper.ToDto(product);
        }

        public Task<bool> ActivateAsync(Guid userId, Guid id, CancellationToken ct = default)
            => ChangeStateAsync(userId, id, activate: true, ct);

        public Task<bool> DeactivateAsync(Guid userId, Guid id, CancellationToken ct = default)
            => ChangeStateAsync(userId, id, activate: false, ct);

        public async Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct = default) // حذف منتج
        {
            var productId = ProductID.From(id);
            var product = await _commandRepository.GetProductAsync(productId, ct);
            if (product is null) return false;

            await EnsureCanManageProductAsync(userId, product, ct);

            var variants = await _commandRepository.GetVariantsByProductAsync(productId, ct);

            _commandRepository.RemoveVariants(variants);
            _commandRepository.RemoveProduct(product);
            await _commandRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<bool> ChangeStateAsync(Guid userId, Guid id, bool activate, CancellationToken ct) // تفعيل أو تعطيل المنتج
        {
            var product = await _commandRepository.GetProductAsync(ProductID.From(id), ct);
            if (product is null) return false;

            await EnsureCanManageProductAsync(userId, product, ct);

            if (activate) product.Activate();
            else product.Deactivate();

            await _commandRepository.SaveChangesAsync(ct);
            return true;
        }

        private async Task EnsureCanManageProductAsync(Guid userId, Product product, CancellationToken ct)
        {
            var category = await _commandRepository.GetMerchantCategoryAsync(product.MerchantCategoryID, ct)
                ?? throw new KeyNotFoundException("Merchant category not found.");

            await _accessService.EnsureCanManageAsync(userId, category.MerchantID, ct);
        }
    }
}