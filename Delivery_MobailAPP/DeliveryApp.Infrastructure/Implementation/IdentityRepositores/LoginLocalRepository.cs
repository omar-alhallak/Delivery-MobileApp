using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.IdentityRepositoresInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Infrastructure.Implementation.IdentityRepositores
{
    public sealed class LoginLocalRepository : ILoginLocalRepository
    {
        private readonly ApplicationDbContext _context;

        public LoginLocalRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // جلب المستخدم عن طريق رقم الهاتف
        public async Task<User?> GetUserByPhoneAsync(string phone, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Phone == phone, ct);
        }

        // جلب طريقة تسجيل الدخول المحلية للمستخدم
        public async Task<UserIdentity?> GetLocalIdentityAsync(UserID userId, CancellationToken ct = default)
        {
            return await _context.UserIdentities.FirstOrDefaultAsync(
                    x => x.UserID == userId && x.Provider == AuthProvider.Local, ct);
        }

        // جلب الجلسة الخاصة بالمستخدم
        public async Task<UserSession?> GetSessionAsync(UserID userId, ClientType clientType, CancellationToken ct = default)
        {
            return await _context.UserSessions.FirstOrDefaultAsync(
                    x => x.UserID == userId && x.ClientType == clientType, ct);
        }

        // إضافة جلسة جديدة للمستخدم
        public async Task AddSessionAsync(UserSession session, CancellationToken ct = default)
        {
            await _context.UserSessions.AddAsync(session, ct);
        }

        // حفظ جميع التغييرات داخل قاعدة البيانات
        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}