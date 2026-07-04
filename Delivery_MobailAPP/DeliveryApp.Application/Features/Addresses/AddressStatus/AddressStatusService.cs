using DeliveryApp.Application.Features.Addresses.Common;
using DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces;
using DeliveryApp.Domain.DomainExceptions;

namespace DeliveryApp.Application.Features.Addresses.AddressStatus
{
    public sealed class AddressStatusService // Use case خاص بالتفعيل والعنوان الافتراضي
    {
        private readonly IAddressCommandRepository _repository; // Repository لتنفيذ أوامر العنوان

        public AddressStatusService(IAddressCommandRepository repository)
        {
            _repository = repository;
        }

        public async Task<AddressDto?> SetDefaultAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // جعل عنوان يملكه المستخدم الحالي default
        {
            var address = await _repository.GetByIdAsync(AddressID.From(id), UserID.From(currentUserId), ct);
            if (address is null) return null;
            if (!address.IsActive) throw new DomainRuleViolationException("Address.Inactive_Cant_Be_Default", "Inactive address cant be default.");

            var userAddresses = await _repository.GetUserAddressesAsync(address.UserID, ct);

            foreach (var userAddress in userAddresses.Where(x => x.ID != address.ID))
            {
                userAddress.RemoveDefault();
            }

            address.SetAsDefault();

            await _repository.SaveChangesAsync(ct);
            return AddressMapper.ToDto(address);
        }

        public async Task<AddressDto?> ActivateAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // إعادة تفعيل عنوان يملكه المستخدم الحالي
        {
            var address = await _repository.GetByIdAsync(AddressID.From(id), UserID.From(currentUserId), ct);
            if (address is null) return null;

            address.Activate();

            await _repository.SaveChangesAsync(ct);
            return AddressMapper.ToDto(address);
        }

        public async Task<AddressDto?> DeactivateAsync(Guid currentUserId, Guid id, CancellationToken ct = default) // تعطيل عنوان يملكه المستخدم الحالي
        {
            var address = await _repository.GetByIdAsync(AddressID.From(id), UserID.From(currentUserId), ct);
            if (address is null) return null;

            address.Deactivate();

            await _repository.SaveChangesAsync(ct);
            return AddressMapper.ToDto(address);
        }
    }
}
