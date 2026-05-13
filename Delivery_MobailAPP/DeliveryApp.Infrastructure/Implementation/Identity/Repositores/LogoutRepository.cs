using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Domain.Enums.IdentityEnums;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.IdentityInterfaces;
using UserID = DeliveryApp.Domain.ValueObjects.StrongID<DeliveryApp.Domain.ValueObjects.UserTag>;

namespace DeliveryApp.Infrastructure.Implementation.Identity.Repositores
{
    public sealed class LogoutRepository : ILogoutRepository
    {
        private readonly ApplicationDbContext _context;

        public LogoutRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // جلب الجلسة الخاصة بالمستخدم
        public async Task<UserSession?> GetSessionAsync(UserID userId, ClientType clientType, CancellationToken ct = default)
        {
            return await _context.UserSessions.FirstOrDefaultAsync(
                    x => x.UserID == userId && x.ClientType == clientType, ct);
        }

        // حفظ جميع التعديلات داخل قاعدة البيانات
        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}