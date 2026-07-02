using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces
{
    public interface IAddressCommandRepository // عقد أوامر العناوين التي تعدل البيانات
    {
        Task<bool> UserExistsAsync(UserID userId, CancellationToken ct = default); // التأكد من وجود المستخدم قبل إنشاء العنوان
        Task<Address?> GetByIdAsync(AddressID addressId, CancellationToken ct = default); // جلب عنوان للتعديل
        Task<IReadOnlyList<Address>> GetUserAddressesAsync(UserID userId, CancellationToken ct = default); // جلب عناوين المستخدم للتعامل مع default
        Task AddAsync(Address address, CancellationToken ct = default); // إضافة عنوان جديد
        Task SaveChangesAsync(CancellationToken ct = default); // حفظ التغييرات
    }
}
