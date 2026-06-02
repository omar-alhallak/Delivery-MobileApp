using DeliveryApp.Domain.Enums.MerchantEnums;

namespace DeliveryApp.Application.Features.Merchants.CreateMerchant
{
    public static class CreateMerchantValidator
    {
        public static CreateMerchantValidatedInput Validate(CreateMerchantRequest request)
        {
            if (request is null)
                throw new Exception("Request is required.");

            // -------------------------
            //        Basic Info
            // -------------------------

            var merchantType = request.MerchantType;

            var merchantName = MerchantInputValidator.ValidateRequiredText(request.MerchantName, nameof(request.MerchantName));

            var slug = MerchantInputValidator.ValidateRequiredText(request.Slug, nameof(request.Slug));

            var description = MerchantInputValidator.ValidateOptionalText(request.Description);

            var phone = MerchantInputValidator.ValidateOptionalText(request.Phone);

            // -------------------------
            //           Photo
            // -------------------------

            var logoUrl = MerchantInputValidator.ValidateOptionalUrl(request.LogoUrl, nameof(request.LogoUrl));

            var coverImageUrl = MerchantInputValidator.ValidateOptionalUrl(request.CoverImageUrl, nameof(request.CoverImageUrl));

            // -------------------------
            //         Location
            // -------------------------

            var latitude = request.Latitude;
            var longitude = request.Longitude;

            // -------------------------
            //     Preparation Time
            // -------------------------

            var defaultPreparationTime = MerchantInputValidator.ToPreparationTime(request.DefaultPreparationMinutes);

            return new CreateMerchantValidatedInput
            (
                merchantType,
                merchantName,
                slug,
                description,
                phone,
                logoUrl,
                coverImageUrl,
                latitude,
                longitude,
                defaultPreparationTime
            );
        }
    }

    public sealed record CreateMerchantValidatedInput
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