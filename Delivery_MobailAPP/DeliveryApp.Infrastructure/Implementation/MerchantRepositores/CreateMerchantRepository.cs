using Microsoft.EntityFrameworkCore;
using DeliveryApp.Domain.Entities.Merchants;
using DeliveryApp.Infrastructure.Persistence;
using DeliveryApp.Application.Interfaces.MerchantRepositoresInterfaces;

namespace DeliveryApp.Infrastructure.Implementation.MerchantRepositores
{
    // Repository مسؤول عن عمليات إنشاء المطعم
    // وظيفته التحقق من وجود Slug
    // وإضافة Merchant و MerchantUser
    // وحفظ التعديلات داخل قاعدة البيانات
    // بدون أي Business Logic

    public sealed class CreateMerchantRepository : ICreateMerchantRepository
    {
        private readonly ApplicationDbContext _context;

        public CreateMerchantRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // التحقق من وجود Slug مسبقاً
        public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct = default)
        {
            return await _context.Merchants.AnyAsync(x => x.Slug.Value == slug, ct);
        }

        // إضافة مطعم جديد
        public async Task AddMerchantAsync(Merchant merchant, CancellationToken ct = default)
        {
            await _context.Merchants.AddAsync(merchant, ct);
        }

        // إضافة مالك المطعم داخل جدول الربط
        public async Task AddMerchantUserAsync(
            MerchantUser merchantUser,
            CancellationToken ct = default)
        {
            await _context.MerchantUsers.AddAsync(merchantUser, ct);
        }

        // حفظ جميع التعديلات داخل قاعدة البيانات
        public async Task SaveChangesAsync(
            CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }
    }
}