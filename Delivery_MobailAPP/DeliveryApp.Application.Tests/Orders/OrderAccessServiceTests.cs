using DeliveryApp.Application.Features.Orders.Access;
using DeliveryApp.Application.Interfaces.OrderRepositoresInterfaces;
using FluentAssertions;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Application.Tests.Orders
{
    public sealed class OrderAccessServiceTests
    {
        [Fact]
        public async Task EnsureMerchantCanManageAsync_WhenRelationIsInactive_Throws()
        {
            var service = new OrderAccessService(new FakeOrderAccessRepository(false));

            Func<Task> action = () => service.EnsureMerchantCanManageAsync(Guid.NewGuid(), Guid.NewGuid());

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task EnsureMerchantCanManageAsync_WhenRelationIsActive_AllowsAccess()
        {
            var service = new OrderAccessService(new FakeOrderAccessRepository(true));

            Func<Task> action = () => service.EnsureMerchantCanManageAsync(Guid.NewGuid(), Guid.NewGuid());

            await action.Should().NotThrowAsync();
        }

        private sealed class FakeOrderAccessRepository : IOrderAccessRepository
        {
            private readonly bool _hasAccess;

            public FakeOrderAccessRepository(bool hasAccess)
            {
                _hasAccess = hasAccess;
            }

            public Task<bool> HasActiveMerchantAccessAsync(UserID userId, MerchantID merchantId, CancellationToken ct = default)
                => Task.FromResult(_hasAccess);
        }
    }
}
