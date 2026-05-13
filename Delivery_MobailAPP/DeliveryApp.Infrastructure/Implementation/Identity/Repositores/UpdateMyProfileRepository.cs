using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Infrastructure.Implementation.Identity.Repositores
{
    public sealed class UpdateMyProfileRepository : IUpdateMyProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public UpdateMyProfileRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // جلب المستخدم عن طريق المعرف
        public async Task<User?> GetUserByIdAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.ID == userId, ct);
        }

        // جلب طريقة الدخول المحلية للمستخدم
        public async Task<UserIdentity?> GetLocalIdentityAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.UserIdentities.FirstOrDefaultAsync(
                    x => x.UserID == userId && x.PasswordHash != null, ct);
        }

        // التحقق من وجود رقم الهاتف لمستخدم آخر
        public async Task<bool> PhoneExistsAsync(string phone, UserID currentUserId, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(x => x.Phone == phone && x.ID != currentUserId, ct);
        }

        // حفظ جميع التعديلات داخل قاعدة البيانات
        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}