using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.GetMerchantDetails
{
    public sealed class GetMerchantDetailsService
    {
        private readonly IGetMerchantDetailsRepository _repository;

        public GetMerchantDetailsService(IGetMerchantDetailsRepository repository)
        {
            _repository = repository
                ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<GetMerchantDetailsResponse> ExecuteAsync(Guid currentUserId, GetMerchantDetailsRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var currentUserID = UserID.From(currentUserId);

            var relation = await _repository.GetMerchantUserAsync(input.MerchantID, currentUserID, ct);

            if (relation is null || !relation.IsActive)
                throw new Exception("Merchant access denied.");

            var merchant = await _repository.GetMerchantDetailsAsync(input.MerchantID, ct);

            if (merchant is null)
                throw new Exception("Merchant not found.");

            return merchant;
        }

        private static GetMerchantDetailsValidatedInput Validate(GetMerchantDetailsRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.MerchantId == Guid.Empty)
                throw new Exception("Merchant id is required.");

            return new GetMerchantDetailsValidatedInput(MerchantID.From(request.MerchantId));
        }

        private sealed record GetMerchantDetailsValidatedInput(MerchantID MerchantID);
    }
}