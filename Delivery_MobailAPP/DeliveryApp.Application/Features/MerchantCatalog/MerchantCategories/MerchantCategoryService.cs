using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Features.MerchantCatalog.Access;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.MerchantCategories
{
    public sealed class MerchantCategoryService // Use case يجمع CRUD تصنيفات المطعم
    {
        private readonly IMerchantCatalogReadRepository _readRepository;
        private readonly IMerchantCatalogCommandRepository _commandRepository;
        private readonly MerchantCatalogAccessService _accessService;

        public MerchantCategoryService(IMerchantCatalogReadRepository readRepository, IMerchantCatalogCommandRepository commandRepository, MerchantCatalogAccessService accessService)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
            _accessService = accessService;
        }

        public async Task<IReadOnlyList<MerchantCategoryDto>> GetByMerchantAsync(Guid userId, Guid merchantId, CancellationToken ct = default) // جلب تصنيفات مطعم
        {
            var merchantID = MerchantID.From(merchantId);
            await _accessService.EnsureCanManageAsync(userId, merchantID, ct);

            var categories = await _readRepository.GetMerchantCategoriesAsync(merchantID, activeOnly: false, ct);
            return categories.Select(CatalogMapper.ToDto).ToList();
        }

        public async Task<MerchantCategoryDto> CreateAsync(Guid userId, Guid merchantId, CreateMerchantCategoryRequest request, CancellationToken ct = default) // إنشاء تصنيف مطعم
        {
            if (request is null) throw new Exception("Merchant category request is required.");

            var merchantID = MerchantID.From(merchantId);
            await _accessService.EnsureCanManageAsync(userId, merchantID, ct);

            var category = new MerchantCategory
            (
                MerchantCategoryID.New(),
                merchantID,
                request.CategoryName,
                request.Description,
                request.ImageUrl,
                request.SortOrder,
                DateTimeOffset.UtcNow
            );

            await _commandRepository.AddMerchantCategoryAsync(category, ct);
            await _commandRepository.SaveChangesAsync(ct);

            return CatalogMapper.ToDto(category);
        }

        public async Task<MerchantCategoryDto?> UpdateAsync(Guid userId, Guid id, UpdateMerchantCategoryRequest request, CancellationToken ct = default) // تعديل تصنيف مطعم
        {
            if (request is null) throw new Exception("Merchant category update request is required.");

            var category = await _commandRepository.GetMerchantCategoryAsync(MerchantCategoryID.From(id), ct);
            if (category is null) return null;

            await _accessService.EnsureCanManageAsync(userId, category.MerchantID, ct);

            category.Rename(request.CategoryName);
            category.ChangeDescription(request.Description);
            category.ChangeImage(request.ImageUrl);
            category.ChangeSortOrder(request.SortOrder);

            await _commandRepository.SaveChangesAsync(ct);
            return CatalogMapper.ToDto(category);
        }

        public Task<bool> ActivateAsync(Guid userId, Guid id, CancellationToken ct = default)
            => ChangeStateAsync(userId, id, activate: true, ct);

        public Task<bool> DeactivateAsync(Guid userId, Guid id, CancellationToken ct = default)
            => ChangeStateAsync(userId, id, activate: false, ct);

        public async Task<bool> DeleteAsync(Guid userId, Guid id, CancellationToken ct = default) // حذف تصنيف مطعم
        {
            var categoryId = MerchantCategoryID.From(id);
            var category = await _commandRepository.GetMerchantCategoryAsync(categoryId, ct);
            if (category is null) return false;

            await _accessService.EnsureCanManageAsync(userId, category.MerchantID, ct);

            var products = await _commandRepository.GetProductsByCategoryAsync(categoryId, ct);
            var variants = await _commandRepository.GetVariantsByProductIdsAsync(products.Select(x => x.ID), ct);

            _commandRepository.RemoveVariants(variants);
            _commandRepository.RemoveProducts(products);
            _commandRepository.RemoveMerchantCategory(category);
            await _commandRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<bool> ChangeStateAsync(Guid userId, Guid id, bool activate, CancellationToken ct) // تفعيل أو تعطيل التصنيف
        {
            var category = await _commandRepository.GetMerchantCategoryAsync(MerchantCategoryID.From(id), ct);
            if (category is null) return false;

            await _accessService.EnsureCanManageAsync(userId, category.MerchantID, ct);

            if (activate) category.Activate();
            else category.Deactivate();

            await _commandRepository.SaveChangesAsync(ct);
            return true;
        }
    }
}