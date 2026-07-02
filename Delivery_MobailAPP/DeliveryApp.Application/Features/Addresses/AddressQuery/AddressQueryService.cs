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

        public async Task<IReadOnlyList<AddressDto>> GetByUserAsync(Guid userId, CancellationToken ct = default) // جلب عناوين مستخدم
        {
            var addresses = await _repository.GetByUserAsync(UserID.From(userId), ct);
            return addresses.Select(AddressMapper.ToDto).ToList();
        }

        public async Task<AddressDto?> GetByIdAsync(Guid id, CancellationToken ct = default) // جلب عنوان واحد
        {
            var address = await _repository.GetByIdAsync(AddressID.From(id), ct);
            return address is null ? null : AddressMapper.ToDto(address);
        }
    }
}
