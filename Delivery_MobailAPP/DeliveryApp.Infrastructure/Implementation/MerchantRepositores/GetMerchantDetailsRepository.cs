using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Features.Merchants.GetMerchantDetails;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;

namespace DeliveryApp.Infrastructure.Implementation.Merchants.Repositores
{
    public sealed class GetMerchantDetailsRepository : IGetMerchantDetailsRepository
    {
        private readonly ApplicationDbContext _context;

        public GetMerchantDetailsRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Task<MerchantUser?> GetMerchantUserAsync(MerchantID merchantId, UserID userId, CancellationToken ct = default)
        {
            return _context.MerchantUsers
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.MerchantID == merchantId && x.UserID == userId, ct);
        }

        public Task<GetMerchantDetailsResponse?> GetMerchantDetailsAsync(MerchantID merchantId, CancellationToken ct = default)
        {
            return _context.Merchants
                .AsNoTracking()
                .Where(x => x.ID == merchantId)
                .Select(x => new GetMerchantDetailsResponse
                {
                    MerchantId = x.ID.Value,

                    PublicId = x.PublicID == null
                        ? null
                        : x.PublicID.Value.Value,

                    MerchantType = x.MerchantType,
                    MerchantName = x.MerchantName.Value,
                    Slug = x.Slug.Value,
                    Description = x.Description,
                    Phone = x.Phone,
                    LogoUrl = x.LogoUrl,
                    CoverImageUrl = x.CoverImageUrl,

                    DefaultPreparationMinutes =
                        (int)x.DefaultPreparationTime.TotalMinutes,

                    Latitude = x.Location.Latitude,
                    Longitude = x.Location.Longitude,

                    AverageRating = x.AverageRating,
                    RatingsCount = x.RatingsCount,
                    IsActive = x.IsActive,
                    CreatedAt = x.CreatedAt
                }).FirstOrDefaultAsync(ct);
        }
    }
}