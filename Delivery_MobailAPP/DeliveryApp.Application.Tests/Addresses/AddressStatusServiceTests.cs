using DeliveryApp.Application.Features.Addresses.AddressStatus;
using DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces;
using DeliveryApp.Domain.Entities.Customers;
using FluentAssertions;
using AddressID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.AddressTag>;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Application.Tests.Addresses
{
    public sealed class AddressStatusServiceTests
    {
        [Fact]
        public async Task SetDefaultAsync_WhenAddressIsActive_UsesRepositorySwitchOperation()
        {
            var userId = UserID.New();
            var address = new Address(AddressID.New(), userId, 33.5138, 36.2765, DateTimeOffset.UtcNow);
            var repository = new FakeAddressCommandRepository(address);
            var service = new AddressStatusService(repository);

            var result = await service.SetDefaultAsync(userId.Value, address.ID.Value);

            result.Should().NotBeNull();
            result!.IsDefault.Should().BeTrue();
            repository.SwitchDefaultCalls.Should().Be(1);
            repository.SaveChangesCalls.Should().Be(0);
        }

        private sealed class FakeAddressCommandRepository : IAddressCommandRepository
        {
            private readonly Address _address;

            public FakeAddressCommandRepository(Address address)
            {
                _address = address;
            }

            public int SwitchDefaultCalls { get; private set; }
            public int SaveChangesCalls { get; private set; }

            public Task<bool> UserExistsAsync(UserID userId, CancellationToken ct = default)
                => Task.FromResult(true);

            public Task<Address?> GetByIdAsync(AddressID addressId, UserID userId, CancellationToken ct = default)
                => Task.FromResult<Address?>(_address.ID == addressId && _address.UserID == userId ? _address : null);

            public Task SwitchDefaultAsync(Address address, CancellationToken ct = default)
            {
                SwitchDefaultCalls++;
                address.SetAsDefault();
                return Task.CompletedTask;
            }

            public Task AddAsync(Address address, CancellationToken ct = default)
                => Task.CompletedTask;

            public Task SaveChangesAsync(CancellationToken ct = default)
            {
                SaveChangesCalls++;
                return Task.CompletedTask;
            }
        }
    }
}
