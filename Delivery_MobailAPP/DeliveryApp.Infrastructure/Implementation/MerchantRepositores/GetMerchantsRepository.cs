using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Enums.MerchantEnums;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Features.Merchants.GetMerchants;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;
using SystemCategoryID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.SystemCategoryTag>;

namespace DeliveryApp.Infrastructure.Implementation.Merchants.Repositores
{
    public sealed class GetMerchantsRepository : IGetMerchantsRepository
    {
        private readonly ApplicationDbContext _context;

        public GetMerchantsRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IReadOnlyList<GetMerchantsResponse>> GetActiveMerchantsAsync(MerchantType merchantType, CancellationToken ct = default)
        {
            return await _context.Merchants
                .Where(x => x.IsActive && x.MerchantType == merchantType)
                .OrderByDescending(x => x.AverageRating)
                .Select(x => new GetMerchantsResponse
                {
                    MerchantId = x.ID.Value,
                    PublicId = x.PublicID!.Value.Value,
                    MerchantName = x.MerchantName.Value,
                    Slug = x.Slug.Value,
                    LogoUrl = x.LogoUrl,
                    CoverImageUrl = x.CoverImageUrl,
                    AverageRating = x.AverageRating,
                    RatingsCount = x.RatingsCount,
                    DefaultPreparationMinutes = (int)x.DefaultPreparationTime.TotalMinutes
                }).ToListAsync(ct);
        }

        public async Task<IReadOnlyList<GetMerchantsResponse>> GetActiveMerchantsBySystemCategoryAsync(MerchantType merchantType, Guid systemCategoryId, CancellationToken ct = default)
        {
            var categoryID = SystemCategoryID.From(systemCategoryId);

            return await (from merchantSystemCategory in _context.MerchantSystemCategories
                          join merchant in _context.Merchants
                              on merchantSystemCategory.MerchantID equals merchant.ID
                          where merchant.IsActive &&
                                merchant.MerchantType == merchantType &&
                                merchantSystemCategory.SystemCategoryID == categoryID
                          orderby merchant.AverageRating descending
                          select new GetMerchantsResponse
                          {
                              MerchantId = merchant.ID.Value,
                              PublicId = merchant.PublicID!.Value.Value,
                              MerchantName = merchant.MerchantName.Value,
                              Slug = merchant.Slug.Value,
                              LogoUrl = merchant.LogoUrl,
                              CoverImageUrl = merchant.CoverImageUrl,
                              AverageRating = merchant.AverageRating,
                              RatingsCount = merchant.RatingsCount,
                              DefaultPreparationMinutes = (int)merchant.DefaultPreparationTime.TotalMinutes
                          }).ToListAsync(ct);
        }
    }
}