using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Application.Interfaces.Services;
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

        public async Task<CreateMerchantResponse> ExecuteAsync(UserID currentUserId, CreateMerchantRequest request, CancellationToken ct = default)
        {
            var now = DateTimeOffset.UtcNow;

            // -------------------------
            //        Validation
            // -------------------------

            var input = CreateMerchantValidator.Validate(request);

            // -------------------------
            //       Check Slug
            // -------------------------

            var slugExists = await _repository.SlugExistsAsync(input.Slug, ct);

            if (slugExists)
                throw new Exception("Slug already exists.");

            // -------------------------
            //        Create Merchant
            // -------------------------

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

            // -------------------------
            //        Public ID
            // -------------------------

            var publicId = await _publicCodeGenerator.GenerateMerchantCodeAsync(ct);

            merchant.AssignPublicID(publicId);
            
            // -------------------------
            //       Merchant Owner
            // -------------------------

            var merchantUser = new MerchantUser
            (
                merchantId: merchant.ID,
                userId: currentUserId,
                role: MerchantUserRole.Owner,
                createdAtUtc: now
            );

            // -------------------------
            //            Save
            // -------------------------

            await _repository.AddMerchantAsync(merchant, ct);

            await _repository.AddMerchantUserAsync(merchantUser, ct);

            await _repository.SaveChangesAsync(ct);

            // -------------------------
            //         Response
            // -------------------------

            return new CreateMerchantResponse
            {
                MerchantId = merchant.ID.Value,
                PublicId = merchant.PublicID!.Value.Value,

                MerchantName = merchant.MerchantName.Value,
                Slug = merchant.Slug.Value,

                MerchantType = merchant.MerchantType,

                IsActive = merchant.IsActive
            };
        }
    }
}