using DeliveryApp.Domain.Entities.Merchants.Catalog;
using DeliveryApp.Application.Features.MerchantCatalog.Access;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.MerchantSystemCategories
{
    public sealed class MerchantSystemCategoryService // Use case ربط وفك ربط المطعم مع تصنيفات النظام
    {
        private readonly IMerchantCatalogCommandRepository _repository; // Repository أوامر الكتالوج
        private readonly MerchantCatalogAccessService _accessService;

        public MerchantSystemCategoryService(IMerchantCatalogCommandRepository repository, MerchantCatalogAccessService accessService)
        {
            _repository = repository;
            _accessService = accessService;
        }

        public async Task<bool> AssignAsync(Guid userId, Guid merchantId, AssignMerchantSystemCategoryRequest request, CancellationToken ct = default) // ربط مطعم مع تصنيف نظام
        {
            if (request is null) throw new Exception("Merchant system category request is required.");

            var merchantStrongId = MerchantID.From(merchantId);
            var systemCategoryId = SystemCategoryID.From(request.SystemCategoryId);

            await _accessService.EnsureCanManageAsync(userId, merchantStrongId, ct);

            var systemCategory = await _repository.GetSystemCategoryAsync(systemCategoryId, ct);
            if (systemCategory is null) return false;

            var existingLink = await _repository.GetMerchantSystemCategoryAsync(merchantStrongId, systemCategoryId, ct);
            if (existingLink is not null) return true;

            var link = new MerchantSystemCategory(merchantStrongId, systemCategoryId, DateTimeOffset.UtcNow);

            await _repository.AddMerchantSystemCategoryAsync(link, ct);
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