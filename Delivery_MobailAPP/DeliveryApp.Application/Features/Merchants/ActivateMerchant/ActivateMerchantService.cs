using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.Merchants.ActivateMerchant
{
    public sealed class ActivateMerchantService
    {
        private readonly IActivateMerchantRepository _repository;

        public ActivateMerchantService(IActivateMerchantRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<ActivateMerchantResponse> ExecuteAsync(Guid currentUserId, ActivateMerchantRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var currentUserID = UserID.From(currentUserId);

            var merchant = await _repository.GetMerchantByIdAsync(input.MerchantID, ct);

            if (merchant is null)
                throw new Exception("Merchant not found.");

            var relation = await _repository.GetMerchantUserAsync(input.MerchantID, currentUserID, ct);

            if (relation is null || !relation.IsActive)
                throw new Exception("Merchant access denied.");

            var workingHoursCount = await _repository.GetWorkingHoursCountAsync(input.MerchantID, ct);

            if (workingHoursCount == 0)
                throw new Exception("Merchant working hours are required.");

            var activeProductsCount = await _repository.GetActiveProductsCountAsync(input.MerchantID, ct);

            if (activeProductsCount < 3)
                throw new Exception("Merchant must have at least 3 active products.");

            merchant.Activate();

            await _repository.SaveChangesAsync(ct);

            return new ActivateMerchantResponse
            {
                MerchantId = merchant.ID.Value,
                IsActive = merchant.IsActive
            };
        }

        private static ActivateMerchantValidatedInput Validate(ActivateMerchantRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.MerchantId == Guid.Empty)
                throw new Exception("Merchant id is required.");

            return new ActivateMerchantValidatedInput(MerchantID.From(request.MerchantId));
        }

        private sealed record ActivateMerchantValidatedInput(MerchantID MerchantID);
    }
}