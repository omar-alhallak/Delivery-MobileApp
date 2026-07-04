using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.SystemCategories
{
    public sealed class SystemCategoryService // Use case يجمع CRUD تصنيفات النظام
    {
        private readonly IMerchantCatalogReadRepository _readRepository; // قراءة التصنيفات
        private readonly IMerchantCatalogCommandRepository _commandRepository; // تعديل التصنيفات

        public SystemCategoryService(IMerchantCatalogReadRepository readRepository, IMerchantCatalogCommandRepository commandRepository)
        {
            _readRepository = readRepository;
            _commandRepository = commandRepository;
        }

        public async Task<IReadOnlyList<SystemCategoryDto>> GetAllAsync(CancellationToken ct = default) // جلب كل تصنيفات النظام
        {
            var categories = await _readRepository.GetSystemCategoriesAsync(ct);
            return categories.Select(CatalogMapper.ToDto).ToList();
        }

        public async Task<SystemCategoryDto> CreateAsync(CreateSystemCategoryRequest request, CancellationToken ct = default) // إنشاء تصنيف نظام
        {
            if (request is null) throw new Exception("System category request is required.");

            var category = new SystemCategory
            (
                SystemCategoryID.New(),
                request.MerchantType,
                request.CategoryName,
                request.Slug,
                request.ImageUrl,
                request.SortOrder,
                DateTimeOffset.UtcNow
            );

            await _commandRepository.AddSystemCategoryAsync(category, ct);
            await _commandRepository.SaveChangesAsync(ct);

            return CatalogMapper.ToDto(category);
        }

        public async Task<SystemCategoryDto?> UpdateAsync(Guid id, UpdateSystemCategoryRequest request, CancellationToken ct = default) // تعديل تصنيف نظام
        {
            if (request is null) throw new Exception("System category update request is required.");

            var category = await _commandRepository.GetSystemCategoryAsync(SystemCategoryID.From(id), ct);
            if (category is null) return null;

            category.Rename(request.CategoryName);
            category.ChangeSlug(request.Slug);
            category.ChangeImage(request.ImageUrl);
            category.ChangeSortOrder(request.SortOrder);

            await _commandRepository.SaveChangesAsync(ct);
            return CatalogMapper.ToDto(category);
        }

        public Task<bool> ActivateAsync(Guid id, CancellationToken ct = default)
            => ChangeStateAsync(id, activate: true, ct);

        public Task<bool> DeactivateAsync(Guid id, CancellationToken ct = default)
            => ChangeStateAsync(id, activate: false, ct);

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default) // حذف تصنيف نظام
        {
            var category = await _commandRepository.GetSystemCategoryAsync(SystemCategoryID.From(id), ct);
            if (category is null) return false;

            _commandRepository.RemoveSystemCategory(category);
            await _commandRepository.SaveChangesAsync(ct);

            return true;
        }

        private async Task<bool> ChangeStateAsync(Guid id, bool activate, CancellationToken ct) // تفعيل أو تعطيل التصنيف
        {
            var category = await _commandRepository.GetSystemCategoryAsync(SystemCategoryID.From(id), ct);
            if (category is null) return false;

            if (activate) category.Activate();
            else category.Deactivate();

            await _commandRepository.SaveChangesAsync(ct);
            return true;
        }
    }
}