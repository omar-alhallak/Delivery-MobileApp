using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Merchants.Catalog;

namespace DeliveryApp.Application.Features.MerchantCatalog.MerchantCategories
{
    public sealed class MerchantCategoryService // Use case يجمع CRUD تصنيفات المطعم
    {
        private readonly IMerchantCatalogReadRepository _readRepository;
        private readonly IMerchantCatalogCommandRepository _commandRepository;

        public MerchantCategoryService(IMerchantCatalogReadRepository readRepository, IMerchantCatalogCommandRepository commandRepository)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
        }

        public async Task<IReadOnlyList<MerchantCategoryDto>> GetByMerchantAsync(Guid merchantId, CancellationToken ct = default) // جلب تصنيفات مطعم
        {
            var categories = await _readRepository.GetMerchantCategoriesAsync(MerchantID.From(merchantId), activeOnly: false, ct);
            return categories.Select(CatalogMapper.ToDto).ToList();
        }

        public async Task<MerchantCategoryDto> CreateAsync(Guid merchantId, CreateMerchantCategoryRequest request, CancellationToken ct = default) // إنشاء تصنيف مطعم
        {
            if (request is null) throw new Exception("Merchant category request is required.");

            var category = new MerchantCategory(
                MerchantCategoryID.New(),
                MerchantID.From(merchantId),
                request.CategoryName,
                request.Description,
                request.ImageUrl,
                request.SortOrder,
                DateTimeOffset.UtcNow);

            await _commandRepository.AddMerchantCategoryAsync(category, ct);
            await _commandRepository.SaveChangesAsync(ct);

            return CatalogMapper.ToDto(category);
        }

        public async Task<MerchantCategoryDto?> UpdateAsync(Guid id, UpdateMerchantCategoryRequest request, CancellationToken ct = default) // تعديل تصنيف مطعم
        {
            if (request is null) throw new Exception("Merchant category update request is required.");

            var category = await _commandRepository.GetMerchantCategoryAsync(MerchantCategoryID.From(id), ct);
            if (category is null) return null;

            category.Rename(request.CategoryName);
            category.ChangeDescription(request.Description);
            category.ChangeImage(request.ImageUrl);
            category.ChangeSortOrder(request.SortOrder);

            await _commandRepository.SaveChangesAsync(ct);
            return CatalogMapper.ToDto(category);
        }

        public Task<bool> ActivateAsync(Guid id, CancellationToken ct = default)
            => ChangeStateAsync(id, activate: true, ct);

        public Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
            => ChangeStateAsync(id, activate: false, ct);

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) // حذف تصنيف مطعم
        {
            var categoryId = MerchantCategoryID.From(id);
            var category = await _commandRepository.GetMerchantCategoryAsync(categoryId, ct);
            if (category is null) return false;

            var products = await _commandRepository.GetProductsByCategoryAsync(categoryId, ct);
            var variants = await _commandRepository.GetVariantsByProductIdsAsync(products.Select(x => x.ID), ct);

            _commandRepository.RemoveVariants(variants);
            _commandRepository.RemoveProducts(products);
            _commandRepository.RemoveMerchantCategory(category);
            await _commandRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<bool> ChangeStateAsync(Guid id, bool activate, CancellationToken ct) // تفعيل أو تعطيل التصنيف
        {
            var category = await _commandRepository.GetMerchantCategoryAsync(MerchantCategoryID.From(id), ct);
            if (category is null) return false;

            if (activate) category.Activate();
            else category.Deactivate();

            await _commandRepository.SaveChangesAsync(ct);
            return true;
        }
    }
}
