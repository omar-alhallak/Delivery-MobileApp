using DeliveryApp.Application.Features.Addresses;
using DeliveryApp.Application.Interfaces.Addresses;
using DeliveryApp.Domain.Entities.Customers;
using DeliveryApp.Domain.Entities.Identity;
namespace DeliveryApp.Application.Features.Addresses;

public class AppUserAddressService
{
    private readonly IAddressRepository _repository;

    public AppUserAddressService(IAddressRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<AddressResponse>> GetListAsync(GetUserAddressRequest request)
    {
        var entities = await _repository.GetByUserIdAsync(request.UserID);
        return entities
            .Select(x => MapToResponse(x))
            .ToList();
    }

    public async Task<AddressResponse> CreateAsync(UserID userId, CreateUserAddressRequest request)
    {
        var now = DateTimeOffset.UtcNow;

        // 1. إنشاء العنوان بالمرحلة الأولى كمؤقت باستخدام الـ Constructor المتاح بالـ Domain
        var addressEntity = new Address(
            AddressID.New(), // توليد المعرف الجديد للعنوان
            userId,
            request.Location.Latitude, // استخراج خطوط الطول والعرض من كائن الـ GeoPoint المرسل
            request.Location.Longitude,
            now
        );

        // 2. استدعاء دالة البزنس من الـ Domain لإكمال باقي البيانات وتحويله لعنوان دائم ومكتمل
        addressEntity.Complete(
            request.Label!,
            request.AddressType!.Value, // تأكد من عمل تحويل أو التعامل مع الـ Nullable Enum حسب كودك
            request.BuildingName!,
            request.Floor!,
            request.DoorInfo!,
            request.Notes
        );

        // لو الـ Request طالب تعيينه كافتراضي مباشرة
        if (request.IsDefault)
        {
            addressEntity.SetAsDefault();
        }

        // 3. حفظ الكيان المكتمل والجاهز عن طريق الـ Repository
        var createdEntity = await _repository.CreateAsync(userId, addressEntity);

        // 4. تحويل الكيان المحفوظ إلى Response وإرجاعه
        return MapToResponse(createdEntity);
    }

    public async Task<AddressResponse?> GetByIdAsync(AddressID addressId)
    {
        var entity = await _repository.GetByIdAsync(addressId);

        if (entity is null)
            return null;

        return MapToResponse(entity);
    }

    public async Task SetAsDefaultAsync(UserID userId, AddressID addressId, CancellationToken ct = default)
    {
        await _repository.SetAsDefaultAsync(userId, addressId, ct);
    }


    public async Task<AddressResponse> UpdateAsync(UserID userId, AddressID addressId, UpdateAddressRequest request, CancellationToken ct = default)
    {
        var address = await _repository.GetByIdAsync(addressId, ct);

        if (address is null || address.UserID != userId)
            throw new KeyNotFoundException("Address not found or unauthorized.");

        address.UpdateDetails(
            request.Label,
            request.AddressType,
            request.BuildingName,
            request.Floor,
            request.DoorInfo,
            request.Notes
        );

        await _repository.UpdateAsync(address,ct);

        return MapToResponse(address);
    }
    public async Task DeleteAsync(UserID userId, AddressID addressId, CancellationToken ct = default)
    {
        // 1. جلب العنوان للتحقق
        var address = await _repository.GetByIdAsync(addressId, ct);

        // 2. التحقق من وجوده وملكيته
        if (address is null || address.UserID != userId)
            throw new KeyNotFoundException("Address not found or unauthorized.");

        // 3. الحذف
        await _repository.DeleteAsync(address, ct);
    }


    private static AddressResponse MapToResponse(Address entity)
    {
        return new AddressResponse
        {
            ID = entity.ID,
            UserID = entity.UserID,
            Label = entity.Label,
            AddressType = entity.AddressType,
            Location = entity.Location,
            BuildingName = entity.BuildingName,
            Floor = entity.Floor,
            DoorInfo = entity.DoorInfo,
            Notes = entity.Notes,
            IsDefault = entity.IsDefault,
            IsTemporary = entity.IsTemporary,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }
}