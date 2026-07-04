using DeliveryApp.Application.Features.Addresses.Common;
using DeliveryApp.Application.Features.Addresses.CompleteAddress;
using DeliveryApp.Application.Features.Addresses.CreateAddressLocation;
using DeliveryApp.Application.Features.Addresses.UpdateAddressDetails;
using DeliveryApp.Application.Features.Addresses.UpdateAddressLocation;
using DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces;
using DeliveryApp.Domain.DomainExceptions;
using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Features.Addresses.AddressLifecycle
{
    public sealed class AddressLifecycleService // Use case يجمع إنشاء العنوان وإكماله وتعديل بياناته
    {
        private readonly IAddressCommandRepository _repository; // Repository لتنفيذ أوامر العناوين

        public AddressLifecycleService(IAddressCommandRepository repository)
        {
            _repository = repository;
        }

        public async Task<AddressDto> CreateLocationAsync(Guid currentUserId, CreateAddressLocationRequest request, CancellationToken ct = default) // إنشاء عنوان مؤقت للمستخدم الحالي
        {
            if (request is null) throw new DomainValidationException("Address.Request_Required", "Address request is required.");

            var userId = UserID.From(currentUserId);
            var userExists = await _repository.UserExistsAsync(userId, ct);
            if (!userExists) throw new DomainRuleViolationException("Address.User_Not_Found", "User was not found.");

            var address = new Address(AddressID.New(), userId, request.Latitude, request.Longitude, DateTimeOffset.UtcNow);

            await _repository.AddAsync(address, ct);
            await _repository.SaveChangesAsync(ct);

            return AddressMapper.ToDto(address);
        }

        public async Task<AddressDto?> CompleteAsync(Guid currentUserId, Guid id, CompleteAddressRequest request, CancellationToken ct = default) // إكمال تفاصيل العنوان المؤقت
        {
            if (request is null) throw new DomainValidationException("Address.Request_Required", "Address request is required.");

            var address = await _repository.GetByIdAsync(AddressID.From(id), UserID.From(currentUserId), ct);
            if (address is null) return null;

            address.Complete(request.Label, request.AddressType, request.BuildingName, request.Floor, request.DoorInfo, request.Notes);

            await _repository.SaveChangesAsync(ct);
            return AddressMapper.ToDto(address);
        }

        public async Task<AddressDto?> UpdateDetailsAsync(Guid currentUserId, Guid id, UpdateAddressDetailsRequest request, CancellationToken ct = default) // تعديل تفاصيل العنوان
        {
            if (request is null) throw new DomainValidationException("Address.Request_Required", "Address request is required.");

            var address = await _repository.GetByIdAsync(AddressID.From(id), UserID.From(currentUserId), ct);
            if (address is null) return null;

            address.UpdateDetails(request.Label, request.AddressType, request.BuildingName, request.Floor, request.DoorInfo, request.Notes);

            await _repository.SaveChangesAsync(ct);
            return AddressMapper.ToDto(address);
        }

        public async Task<AddressDto?> UpdateLocationAsync(Guid currentUserId, Guid id, UpdateAddressLocationRequest request, CancellationToken ct = default) // تعديل الموقع قبل إكمال العنوان فقط
        {
            if (request is null) throw new DomainValidationException("Address.Request_Required", "Address request is required.");

            var address = await _repository.GetByIdAsync(AddressID.From(id), UserID.From(currentUserId), ct);
            if (address is null) return null;

            address.Relocate(request.Latitude, request.Longitude);

            await _repository.SaveChangesAsync(ct);
            return AddressMapper.ToDto(address);
        }
    }
}
