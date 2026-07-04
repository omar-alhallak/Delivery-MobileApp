using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces
{
    public interface IAddressReadRepository // عقد قراءة العناوين من دون تعديل
    {
        Task<IReadOnlyList<Address>> GetByUserAsync(UserID userId, CancellationToken ct = default); // جلب عناوين مستخدم
        Task<Address?> GetByIdAsync(AddressID addressId, UserID userId, CancellationToken ct = default); // جلب عنوان يملكه المستخدم الحالي
    }
}
