using Microsoft.EntityFrameworkCore;
using DeliveryApp.Application.Interfaces;
using DeliveryApp.Domain.Entities.Identity;
using DeliveryApp.Infrastructure.Persistence;

namespace DeliveryApp.Infrastructure.Implementation.IdentityRepositores
{
    public sealed class RegisterLocalRepository : IRegisterLocalRepository
    {
        private readonly ApplicationDbContext _context;

        public RegisterLocalRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // التحقق من وجود رقم الهاتف لمستخدم آخر
        public async Task<bool> PhoneExistsAsync(string phone, CancellationToken ct = default)
        {
            return await _context.Users.AnyAsync(x => x.Phone == phone, ct);
        }

        // إضافة مستخدم جديد
        public async Task AddUserAsync(User user, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(user, ct);
        }

        // إضافة طريقة تسجيل دخول للمستخدم
        public async Task AddUserIdentityAsync(UserIdentity identity, CancellationToken ct = default)
        {
            await _context.UserIdentities.AddAsync(identity, ct);
        }

        // حفظ جميع التعديلات داخل قاعدة البيانات
        public async Task SaveChangesAsync(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}