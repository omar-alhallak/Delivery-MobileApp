using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Application.Features.MerchantCatalog.Access;
using DeliveryApp.Application.Features.MerchantCatalog.Common;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.MerchantSystemCategories
{
    public sealed class MerchantSystemCategoryService // Use case ربط وفك ربط المطعم مع تصنيفات النظام
    {
        private readonly IMerchantCatalogCommandRepository _repository; // Repository أوامر الكتالوج
        private readonly IMerchantCatalogReadRepository _readRepository; // Repository قراءة الكتالوج
        private readonly MerchantCatalogAccessService _accessService;

        public MerchantSystemCategoryService(IMerchantCatalogCommandRepository repository,
            IMerchantCatalogReadRepository readRepository, MerchantCatalogAccessService accessService)
        {
            _repository = repository;
            _readRepository = readRepository;
            _accessService = accessService;
        }

        public async Task<IReadOnlyList<SystemCategoryDto>> GetByMerchantAsync(Guid userId, Guid merchantId, CancellationToken ct = default) // جلب تصنيفات النظام المربوطة بالمطعم
        {
            var merchantStrongId = MerchantID.From(merchantId);

            await _accessService.EnsureCanManageAsync(userId, merchantStrongId, ct);

            var categories = await _readRepository.GetSystemCategoriesByMerchantAsync(merchantStrongId, ct);

            return categories.Select(CatalogMapper.ToDto).ToList();
        }

        public async Task<bool> AssignAsync(Guid userId, Guid merchantId, AssignMerchantSystemCategoryRequest request, CancellationToken ct = default) // ربط مطعم مع عدة تصنيفات نظام
        {
            if (request is null) throw new Exception("Merchant system category request is required.");

            if (request.SystemCategoryIds.Count == 0)
                throw new Exception("At least one system category is required.");

            if (request.SystemCategoryIds.Count > 30)
                throw new Exception("Merchant can be linked to maximum 30 system categories.");

            if (request.SystemCategoryIds.Distinct().Count() != request.SystemCategoryIds.Count)
                throw new Exception("System categories cannot be duplicated.");

            var merchantStrongId = MerchantID.From(merchantId);

            await _accessService.EnsureCanManageAsync(userId, merchantStrongId, ct);

            foreach (var id in request.SystemCategoryIds)
            {
                var systemCategoryId = SystemCategoryID.From(id);

                var systemCategory = await _repository.GetSystemCategoryAsync(systemCategoryId, ct);
                if (systemCategory is null) return false;

                var existingLink = await _repository.GetMerchantSystemCategoryAsync(merchantStrongId, systemCategoryId, ct);
                if (existingLink is not null) continue;

                var link = new MerchantSystemCategory(merchantStrongId, systemCategoryId, DateTimeOffset.UtcNow);

                await _repository.AddMerchantSystemCategoryAsync(link, ct);
            }

            await _repository.SaveChangesAsync(ct);

            return true;
        }
       

        public async Task<bool> RemoveAsync(Guid userId, Guid merchantId, Guid systemCategoryId, CancellationToken ct = default) // فك ربط مطعم من تصنيف نظام
        {
            var merchantStrongId = MerchantID.From(merchantId);
            var categoryStrongId = SystemCategoryID.From(systemCategoryId);

            await _accessService.EnsureCanManageAsync(userId, merchantStrongId, ct);

            var link = await _repository.GetMerchantSystemCategoryAsync(merchantStrongId, categoryStrongId, ct);
            if (link is null) return false;

            _repository.RemoveMerchantSystemCategory(link);
            await _repository.SaveChangesAsync(ct);

            return true;
        }
    }
}
