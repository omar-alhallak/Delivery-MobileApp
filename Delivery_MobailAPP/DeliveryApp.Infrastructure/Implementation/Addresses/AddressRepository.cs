using DeliveryApp.Application.Interfaces.Addresses;
using DeliveryApp.Domain.Entities.Customers;
using DeliveryApp.Domain.ValueObjects;
using DeliveryApp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Implementation.Addresses;

public class AddressRepository : IAddressRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AddressRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Address>> GetByUserIdAsync(StrongID<UserTag> userId, CancellationToken ct = default)
    {
        var query = _dbContext.Addresses
            .Where(x => x.UserID == userId);

        return await query.ToListAsync(cancellationToken: ct);
    }

    public async Task<Address> CreateAsync(StrongID<UserTag> userId, Address address, CancellationToken ct = default)
    {
        var result = await _dbContext.Addresses.AddAsync(address, ct);
        await _dbContext.SaveChangesAsync(ct);

        return result.Entity;
    }

    public async Task<Address?> GetByIdAsync(StrongID<AddressTag> id, CancellationToken ct = default)
    {
        return await _dbContext
            .Addresses
            .FirstOrDefaultAsync(x => x.ID == id, ct);
    }

    public async Task SetAsDefaultAsync(StrongID<UserTag> userId, StrongID<AddressTag> id,
        CancellationToken ct = default)
    {
        await _dbContext.Addresses
            .Where(x => x.UserID == userId)
            .ExecuteUpdateAsync(setters
                => setters
                    .SetProperty(p => p.IsDefault, false), ct);

        await _dbContext.Addresses
            .Where(x => x.UserID == userId && x.ID == id)
            .ExecuteUpdateAsync(setters
                => setters
                    .SetProperty(p => p.IsDefault, true), ct);
    }

    public async Task<Address> UpdateAsync(Address address, CancellationToken ct = default)
    {
        if (_dbContext.Addresses.Local.All(e => e != address))
        {
            _dbContext.Addresses.Attach(address);
            _dbContext.Update(address);


            await _dbContext.SaveChangesAsync(ct);
        }

        return address;
    }

    public async Task<Address> DeleteAsync(Address address, CancellationToken ct = default)
    {
        _dbContext.Addresses.Remove(address);
        await _dbContext.SaveChangesAsync(ct);
        return address;
    }


}