using DeliveryApp.Application.Interfaces.Services;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Application.Features.Merchants.CreateMerchant
{
    public sealed class CreateMerchantService
    {
        private readonly ICreateMerchantRepository _repository;
        private readonly IPublicCodeGenerator _publicCodeGenerator;

        public CreateMerchantService(ICreateMerchantRepository repository, IPublicCodeGenerator publicCodeGenerator)
        {
            _repository = repository;
            _publicCodeGenerator = publicCodeGenerator;
        }

        public async Task<CreateMerchantResponse> ExecuteAsync(Guid currentUserId, CreateMerchantRequest request, CancellationToken ct = default)
        {
            var now = DateTimeOffset.UtcNow;
            var userID = UserID.From(currentUserId);

            var input = Validate(request);

            var slugExists = await _repository.SlugExistsAsync(input.Slug, ct);

            if (slugExists) throw new Exception("Slug already exists.");

            var merchant = new Merchant
            (
                id: MerchantID.New(),
                merchantType: input.MerchantType,
                merchantName: input.MerchantName,
                slug: input.Slug,
                lat: input.Latitude,
                lng: input.Longitude,
                description: input.Description,
                phone: input.Phone,
                logoUrl: input.LogoUrl,
                coverImageUrl: input.CoverImageUrl,
                defaultPreparationTime: input.DefaultPreparationTime,
                createdAtUtc: now
            );

            var publicId = await _publicCodeGenerator.GenerateMerchantCodeAsync(ct);
            merchant.AssignPublicID(publicId);

            var merchantUser = new MerchantUser
            (
                merchantId: merchant.ID,
                userId: userID,
                role: MerchantUserRole.Owner,
                createdAtUtc: now
            );

            await _repository.AddMerchantAsync(merchant, ct);
            await _repository.AddMerchantUserAsync(merchantUser, ct);
            await _repository.SaveChangesAsync(ct);

            return new CreateMerchantResponse
            {
                MerchantId = merchant.ID.Value,
                PublicId = merchant.PublicID!.Value.Value,
                IsActive = merchant.IsActive
            };
        }

        private static CreateMerchantValidatedInput Validate(CreateMerchantRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            if (!Enum.IsDefined(typeof(MerchantType), request.MerchantType))
                throw new Exception("Invalid merchant type.");

            var merchantName = RequiredText(request.MerchantName, nameof(request.MerchantName));
            var slug = RequiredText(request.Slug, nameof(request.Slug));

            var description = OptionalText(request.Description);
            var phone = OptionalText(request.Phone);

            var logoUrl = OptionalUrl(request.LogoUrl, nameof(request.LogoUrl));
            var coverImageUrl = OptionalUrl(request.CoverImageUrl, nameof(request.CoverImageUrl));

            var preparationTime = TimeSpan.FromMinutes(request.DefaultPreparationMinutes);

            return new CreateMerchantValidatedInput
            (
                request.MerchantType,
                merchantName,
                slug,
                description,
                phone,
                logoUrl,
                coverImageUrl,
                request.Latitude,
                request.Longitude,
                preparationTime
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
                throw new Exception($"Invalid {fieldName} URL.");

            return value;
        }

        private sealed record CreateMerchantValidatedInput
        (
            MerchantType MerchantType,
            string MerchantName,
            string Slug,
            string? Description,
            string? Phone,
            string? LogoUrl,
            string? CoverImageUrl,
            double Latitude,
            double Longitude,
            TimeSpan DefaultPreparationTime
        );
    }
}