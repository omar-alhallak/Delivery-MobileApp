using DeliveryApp.Application.Features.Addresses.Common;
using DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces;

namespace DeliveryApp.Application.Features.Addresses.AddressQuery
{
    public sealed class AddressQueryService // Use case خاص بقراءة العناوين
    {
        private readonly IAddressReadRepository _repository; // Repository للقراءة فقط

        public AddressQueryService(IAddressReadRepository repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyList<AddressDto>> GetMineAsync(Guid currentUserId, CancellationToken ct = default) // جلب عناوين المستخدم الحالي
        {
            var addresses = await _repository.GetByUserAsync(UserID.From(currentUserId), ct);
            return addresses.Select(AddressMapper.ToDto).ToList();
        }

        public async Task<AddressDto?> GetByIdAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // جلب عنوان يملكه المستخدم الحالي
        {
            var address = await _repository.GetByIdAsync(AddressID.From(id), UserID.From(currentUserId), ct);
            return address is null ? null : AddressMapper.ToDto(address);
        }
    }
}