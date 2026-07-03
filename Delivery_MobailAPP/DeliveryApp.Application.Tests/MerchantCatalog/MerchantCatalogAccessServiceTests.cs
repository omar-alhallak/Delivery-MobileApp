using DeliveryApp.Application.Features.MerchantCatalog.Access;
using DeliveryApp.Application.Interfaces.MerchantCatalogRepositoriesInterfaces;
using DeliveryApp.Domain.Enums.MerchantEnums;
using FluentAssertions;
using MerchantID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.MerchantTag>;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Application.Tests.MerchantCatalog
{
    public sealed class MerchantCatalogAccessServiceTests
    {
        [Fact]
        public async Task EnsureCanManageAsync_WhenRelationIsNotActive_ThrowsUnauthorizedAccessException()
        {
            var repository = new FakeAccessRepository { HasAccess = false };
            var service = new MerchantCatalogAccessService(repository);

            Func<Task> action = () => service.EnsureCanManageAsync(Guid.NewGuid(), MerchantID.New());

            await action.Should().ThrowAsync<UnauthorizedAccessException>();
        }

        [Fact]
        public async Task EnsureCanManageAsync_WhenRelationIsActive_AllowsAccess()
        {
            var repository = new FakeAccessRepository { HasAccess = true };
            var service = new MerchantCatalogAccessService(repository);

            Func<Task> action = () => service.EnsureCanManageAsync(Guid.NewGuid(), MerchantID.New());

            await action.Should().NotThrowAsync();
        }

        [Fact]
        public async Task GetMyMerchantsAsync_ReturnsOnlyRepositoryResult()
        {
            var expected = new List<MyMerchantDto>
            {
                new()
                {
                    MerchantId = Guid.NewGuid(),
                    MerchantName = "Test Merchant",
                    Role = MerchantUserRole.Staff,
                    IsActive = true
                }
            };
            var repository = new FakeAccessRepository { Merchants = expected };
            var service = new MerchantCatalogAccessService(repository);

            var result = await service.GetMyMerchantsAsync(Guid.NewGuid());

            result.Should().BeEquivalentTo(expected);
        }

        private sealed class FakeAccessRepository : IMerchantCatalogAccessRepository
        {
            public bool HasAccess { get; init; }
            public IReadOnlyList<MyMerchantDto> Merchants { get; init; } = [];

            public Task<bool> HasActiveAccessAsync(UserID userId, MerchantID merchantId, CancellationToken ct = default)
                => Task.FromResult(HasAccess);

            public Task<IReadOnlyList<MyMerchantDto>> GetUserMerchantsAsync(UserID userId, CancellationToken ct = default)
                => Task.FromResult(Merchants);
        }
    }
}
