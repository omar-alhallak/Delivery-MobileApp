using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Features.MerchantCatalog.Access;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.Variants
{
    public sealed class VariantService // Use case يجمع CRUD خيارات المنتجات
    {
        private readonly IMerchantCatalogReadRepository _readRepository;
        private readonly IMerchantCatalogCommandRepository _commandRepository;
        private readonly MerchantCatalogAccessService _accessService;

        public VariantService(IMerchantCatalogReadRepository readRepository, IMerchantCatalogCommandRepository commandRepository,
            MerchantCatalogAccessService accessService)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
            _accessService = accessService;
        }

        public async Task<IReadOnlyList<VariantDto>> GetByProductAsync(Guid userId, Guid productId, CancellationToken ct = default) // جلب خيارات منتج
        {
            var productID = ProductID.From(productId);
            var product = await _commandRepository.GetProductAsync(productID, ct);
            if (product is null) return [];

            await EnsureCanManageProductAsync(userId, product, ct);

            var variants = await _readRepository.GetVariantsByProductAsync(productID, activeOnly: false, ct);
            return variants.Select(CatalogMapper.ToDto).ToList();
        }

        public async Task<VariantDto> CreateAsync(Guid userId, Guid productId, CreateVariantRequest request, CancellationToken ct = default) // إنشاء خيار
        {
            if (request is null) throw new Exception("Variant request is required.");

            var productID = ProductID.From(productId);
            var product = await _commandRepository.GetProductAsync(productID, ct);
            if (product is null) throw new KeyNotFoundException("Product not found.");

            await EnsureCanManageProductAsync(userId, product, ct);

            var variant = new Variant
            (
                VariantID.New(),
                productID,
                request.VariantName,
                request.SortOrder,
                request.BasePrice,
                DateTimeOffset.UtcNow
            );

            await _commandRepository.AddVariantAsync(variant, ct);
            await _commandRepository.SaveChangesAsync(ct);

            return CatalogMapper.ToDto(variant);
        }

        public async Task<VariantDto?> UpdateAsync(Guid userId, Guid id, UpdateVariantRequest request, CancellationToken ct = default) // تعديل اسم أو سعر الخيار
        {
            if (request is null) throw new Exception("Variant update request is required.");

            var variant = await _commandRepository.GetVariantAsync(VariantID.From(id), ct);
            if (variant is null) return null;

            await EnsureCanManageVariantAsync(userId, variant, ct);

            variant.Rename(request.VariantName);
            variant.ChangePrice(request.BasePrice);
            variant.ChangeSortOrder(request.SortOrder);

            await _commandRepository.SaveChangesAsync(ct);
            return CatalogMapper.ToDto(variant);
        }

        public Task<bool> ActivateAsync(Guid userId, Guid id, CancellationToken ct = default)
            => ChangeStateAsync(userId, id, activate: true, ct);

        public Task<bool> DeactivateAsync(Guid userId, Guid id, CancellationToken ct = default)
            => ChangeStateAsync(userId, id, activate: false, ct);

        public async Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct = default) // حذف خيار
        {
            var variant = await _commandRepository.GetVariantAsync(VariantID.From(id), ct);
            if (variant is null) return false;

            await EnsureCanManageVariantAsync(userId, variant, ct);

            _commandRepository.RemoveVariant(variant);
            await _commandRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<bool> ChangeStateAsync(Guid userId, Guid id, bool activate, CancellationToken ct) // تفعيل أو تعطيل الخيار
        {
            var variant = await _commandRepository.GetVariantAsync(VariantID.From(id), ct);
            if (variant is null) return false;

            await EnsureCanManageVariantAsync(userId, variant, ct);

            if (activate) variant.Activate();
            else variant.Deactivate();

            await _commandRepository.SaveChangesAsync(ct);
            return true;
        }

        private async Task EnsureCanManageVariantAsync(Guid userId, Variant variant, CancellationToken ct)
        {
            var product = await _commandRepository.GetProductAsync(variant.ProductID, ct)
                ?? throw new KeyNotFoundException("Product not found.");

            await EnsureCanManageProductAsync(userId, product, ct);
        }

        private async Task EnsureCanManageProductAsync(Guid userId, Product product, CancellationToken ct)
        {
            var category = await _commandRepository.GetMerchantCategoryAsync(product.MerchantCategoryID, ct)
                ?? throw new KeyNotFoundException("Merchant category not found.");

            await _accessService.EnsureCanManageAsync(userId, category.MerchantID, ct);
        }
    }
}