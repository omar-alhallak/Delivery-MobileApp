using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.GetMerchants
{
    public sealed class GetMerchantsService
    {
        private readonly IGetMerchantsRepository _repository;

        public GetMerchantsService(IGetMerchantsRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<IReadOnlyList<GetMerchantsResponse>> ExecuteAsync(GetMerchantsRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var merchants = input.SystemCategoryId is null
                ? await _repository.GetActiveMerchantsAsync(input.MerchantType, ct)
                : await _repository.GetActiveMerchantsBySystemCategoryAsync(
                    input.MerchantType,
                    input.SystemCategoryId.Value,
                    ct);

            return merchants;
        }

        private static GetMerchantsValidatedInput Validate(
            GetMerchantsRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (!Enum.IsDefined(typeof(MerchantType), request.MerchantType))
                throw new Exception("Invalid merchant type.");

            if (request.SystemCategoryId.HasValue &&
                request.SystemCategoryId.Value == Guid.Empty)
                throw new Exception("System category id is invalid.");

            return new GetMerchantsValidatedInput(
                request.MerchantType,
                request.SystemCategoryId);
        }

        private sealed record GetMerchantsValidatedInput(MerchantType MerchantType,Guid? SystemCategoryId);
    }
}