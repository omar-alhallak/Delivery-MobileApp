using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Interfaces.Addresses;

public interface IAddressRepository
{
    Task<IReadOnlyList<Address>> GetByUserIdAsync(UserID userId, CancellationToken ct = default);
    
    Task<Address> CreateAsync(UserID userId, Address address, CancellationToken ct = default);
    
    Task<Address?> GetByIdAsync(AddressID id, CancellationToken ct = default);
    
    Task SetAsDefaultAsync(UserID userId, AddressID id, CancellationToken ct = default);

    Task <Address>UpdateAsync(Address address,CancellationToken ct = default);
    Task <Address> DeleteAsync(Address address,CancellationToken ct = default);
}