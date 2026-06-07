using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Merchants.Catalog;

namespace DeliveryApp.Application.Features.MerchantCatalog.Variants
{
    public sealed class VariantService // Use case يجمع CRUD خيارات المنتجات
    {
        private readonly IMerchantCatalogReadRepository _readRepository;
        private readonly IMerchantCatalogCommandRepository _commandRepository;

        public VariantService(IMerchantCatalogReadRepository readRepository, IMerchantCatalogCommandRepository commandRepository)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
        }

        public async Task<IReadOnlyList<VariantDto>> GetByProductAsync(Guid productId, CancellationToken ct = default) // جلب خيارات منتج
        {
            var variants = await _readRepository.GetVariantsByProductAsync(ProductID.From(productId), activeOnly: false, ct);
            return variants.Select(CatalogMapper.ToDto).ToList();
        }

        public async Task<VariantDto> CreateAsync(Guid productId, CreateVariantRequest request, CancellationToken ct = default) // إنشاء خيار
        {
            if (request is null) throw new Exception("Variant request is required.");

            var variant = new Variant(
                VariantID.New(),
                ProductID.From(productId),
                request.VariantName,
                request.SortOrder,
                request.BasePrice,
                DateTimeOffset.UtcNow);

            await _commandRepository.AddVariantAsync(variant, ct);
            await _commandRepository.SaveChangesAsync(ct);

            return CatalogMapper.ToDto(variant);
        }

        public async Task<VariantDto?> UpdateAsync(Guid id, UpdateVariantRequest request, CancellationToken ct = default) // تعديل اسم أو سعر الخيار
        {
            if (request is null) throw new Exception("Variant update request is required.");

            var variant = await _commandRepository.GetVariantAsync(VariantID.From(id), ct);
            if (variant is null) return null;

            variant.Rename(request.VariantName);
            variant.ChangePrice(request.BasePrice);
            variant.ChangeSortOrder(request.SortOrder);

            await _commandRepository.SaveChangesAsync(ct);
            return CatalogMapper.ToDto(variant);
        }

        public Task<bool> ActivateAsync(Guid id, CancellationToken ct = default)
            => ChangeStateAsync(id, activate: true, ct);

        public Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
            => ChangeStateAsync(id, activate: false, ct);

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) // حذف خيار
        {
            var variant = await _commandRepository.GetVariantAsync(VariantID.From(id), ct);
            if (variant is null) return false;

            _commandRepository.RemoveVariant(variant);
            await _commandRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<bool> ChangeStateAsync(Guid id, bool activate, CancellationToken ct) // تفعيل أو تعطيل الخيار
        {
            var variant = await _commandRepository.GetVariantAsync(VariantID.From(id), ct);
            if (variant is null) return false;

            if (activate) variant.Activate();
            else variant.Deactivate();

            await _commandRepository.SaveChangesAsync(ct);
            return true;
        }
    }
}
