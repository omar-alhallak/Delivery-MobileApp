using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.MerchantCatalog.Access
{
    public sealed class MerchantCatalogAccessService // يتحقق أن المستخدم مرتبط بالمطعم بعلاقة فعالة
    {
        private readonly IMerchantCatalogAccessRepository _repository;

        public MerchantCatalogAccessService(IMerchantCatalogAccessRepository repository)
        {
            _repository = repository;
        }

        public async Task EnsureCanManageAsync(Guid userId, MerchantID merchantId, CancellationToken ct = default)
        {
            var hasAccess = await _repository.HasActiveAccessAsync(UserID.From(userId), merchantId, ct);

            if (!hasAccess)
                throw new UnauthorizedAccessException("Merchant access denied.");
        }

        public Task<IReadOnlyList<MyMerchantDto>> GetMyMerchantsAsync(Guid userId, CancellationToken ct = default)
            => _repository.GetUserMerchantsAsync(UserID.From(userId), ct);
    }
}