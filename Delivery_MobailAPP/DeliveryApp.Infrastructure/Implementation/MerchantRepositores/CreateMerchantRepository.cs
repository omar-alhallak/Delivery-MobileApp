using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Infrastructure.Implementation.Merchants.Repositores
{
    public sealed class CreateMerchantRepository : ICreateMerchantRepository
    {
        private readonly ApplicationDbContext _context;

        public CreateMerchantRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
        {
            return await _context.Merchants.AnyAsync(x => x.Slug == Slug.Create(slug), ct);
        }

        public async Task AddMerchantAsync(Merchant merchant, CancellationToken ct = default)
        {
            await _context.Merchants.AddAsync(merchant, ct);
        }

        public async Task AddMerchantUserAsync(MerchantUser merchantUser, CancellationToken ct = default)
        {
            await _context.MerchantUsers.AddAsync(merchantUser, ct);
        }

        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}