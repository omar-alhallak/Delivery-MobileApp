using DeliveryApp.Domain.Entities.Customers;

namespace DeliveryApp.Application.Interfaces.AddressRepositoriesInterfaces
{
    public interface IAddressCommandRepository // عقد أوامر العناوين التي تعدل البيانات
    {
        Task<bool> UserExistsAsync(UserID userId, CancellationToken ct = default); // التأكد من وجود المستخدم قبل إنشاء العنوان
        Task<Address?> GetByIdAsync(AddressID addressId, UserID userId, CancellationToken ct = default); // جلب عنوان يملكه المستخدم الحالي للتعديل
        Task SwitchDefaultAsync(Address address, CancellationToken ct = default); // إلغاء العنوان الافتراضي القديم وتعيين الجديد ضمن عملية واحدة
        Task AddAsync(Address address, CancellationToken ct = default); // إضافة عنوان جديد
        Task SaveChangesAsync(CancellationToken ct = default); // حفظ التغييرات
    }
}
