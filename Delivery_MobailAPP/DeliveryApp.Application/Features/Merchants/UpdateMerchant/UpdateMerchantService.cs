using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.UpdateMerchant
{
    public sealed class UpdateMerchantService
    {
        private readonly IUpdateMerchantRepository _repository;

        public UpdateMerchantService(IUpdateMerchantRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<UpdateMerchantResponse> ExecuteAsync(Guid currentUserId, UpdateMerchantRequest request, CancellationToken ct = default)
        {
            var input = Validate(request);

            var currentUserID = UserID.From(currentUserId);

            var merchant = await _repository.GetMerchantByIdAsync(input.MerchantID, ct);

            if (merchant is null)
                throw new Exception("Merchant not found.");

            var relation = await _repository.GetMerchantUserAsync(input.MerchantID, currentUserID, ct);

            if (relation is null || !relation.IsActive)
                throw new Exception("Merchant access denied.");

            if (relation.Role != MerchantUserRole.Owner)
                throw new Exception("Only owner can update merchant.");

            if (input.MerchantName is not null)
                merchant.Rename(input.MerchantName);

            if (input.Slug is not null)
            {
                var slugExists = await _repository.SlugExistsAsync(input.Slug, input.MerchantID, ct);

                if (slugExists) throw new Exception("Slug already exists.");

                merchant.ChangeSlug(input.Slug);
            }

            if (input.Description is not null)
                merchant.ChangeDescription(input.Description);

            if (input.Phone is not null)
                merchant.ChangePhone(input.Phone);

            if (input.LogoUrl is not null)
                merchant.ChangeLogo(input.LogoUrl);

            if (input.CoverImageUrl is not null)
                merchant.ChangeCoverImage(input.CoverImageUrl);

            if (input.Latitude.HasValue && input.Longitude.HasValue)
                merchant.Relocate(input.Latitude.Value, input.Longitude.Value);

            if (input.DefaultPreparationTime.HasValue)
                merchant.ChangeDefaultPreparationTime(input.DefaultPreparationTime.Value);

            await _repository.SaveChangesAsync(ct);

            return new UpdateMerchantResponse
            {
                MerchantId = merchant.ID.Value,
                PublicId = merchant.PublicID!.Value.Value,
                IsActive = merchant.IsActive
            };
        }

        private static UpdateMerchantValidatedInput Validate(UpdateMerchantRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (request.MerchantId == Guid.Empty)
                throw new Exception("Merchant id is required.");

            var merchantID = MerchantID.From(request.MerchantId);

            var merchantName = request.MerchantName is null ? null : RequiredText(request.MerchantName, nameof(request.MerchantName));
            var slug = request.Slug is null ? null : RequiredText(request.Slug, nameof(request.Slug));

            var description = request.Description is null ? null : OptionalText(request.Description);

            var phone = request.Phone is null ? null : OptionalText(request.Phone);

            var logoUrl = request.LogoUrl is null ? null : OptionalUrl(request.LogoUrl, nameof(request.LogoUrl));

            var coverImageUrl = request.CoverImageUrl is null ? null : OptionalUrl(request.CoverImageUrl, nameof(request.CoverImageUrl));

            if (request.Latitude.HasValue != request.Longitude.HasValue)
                throw new Exception("Latitude and longitude must be sent together.");

            var defaultPreparationTime = request.DefaultPreparationMinutes.HasValue
                ? TimeSpan.FromMinutes(request.DefaultPreparationMinutes.Value)
                : (TimeSpan?)null;

            return new UpdateMerchantValidatedInput
            (
                merchantID,
                merchantName,
                slug,
                description,
                phone,
                logoUrl,
                coverImageUrl,
                request.Latitude,
                request.Longitude,
                defaultPreparationTime
            );
        }

        private static string RequiredText(string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new Exception($"{fieldName} is required.");

            return value.Trim();
        }

        private static string? OptionalText(string? value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        private static string? OptionalUrl(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            value = value.Trim();

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)) 
            {
                throw new Exception($"Invalid {fieldName} URL.");
            }

            return value;
        }

        private sealed record UpdateMerchantValidatedInput
        (
            MerchantID MerchantID,
            string? MerchantName,
            string? Slug,
            string? Description,
            string? Phone,
            string? LogoUrl,
            string? CoverImageUrl,
            double? Latitude,
            double? Longitude,
            TimeSpan? DefaultPreparationTime
        );
    }
}
